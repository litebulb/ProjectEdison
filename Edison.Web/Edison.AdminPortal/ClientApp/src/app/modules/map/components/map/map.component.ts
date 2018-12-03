import { Observable, Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

import {
    AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit,
    Output, ViewChild
} from '@angular/core';
import { select, Store } from '@ngrx/store';

import { environment } from '../../../../../environments/environment';
import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { fadeInOutHalfOpacity } from '../../../../core/animations/fadeInOutHalfOpacity';
import { getRankingColor } from '../../../../core/colorRank';
import { GeoLocation } from '../../../../core/models/geoLocation';
import { spinnerColors } from '../../../../core/spinnerColors';
import { AppState } from '../../../../reducers';
import {
    selectingActionPlanSelector
} from '../../../../reducers/action-plan/action-plan.selectors';
import { ShowEventInEventBar } from '../../../../reducers/event/event.actions';
import { Event, EventType } from '../../../../reducers/event/event.model';
import {
    AddLocationToActiveResponse, ShowSelectingLocation, UpdateResponse
} from '../../../../reducers/response/response.actions';
import { Response } from '../../../../reducers/response/response.model';
import {
    activeResponseSelector, showSelectingLocationSelector
} from '../../../../reducers/response/response.selectors';
import {
    CircleSpinnerComponent
} from '../../../shared/components/circle-spinner/circle-spinner.component';
import { MapClick } from '../../models/mapClick';
import { MapDefaults } from '../../models/mapDefaults';
import { MapPin } from '../../models/mapPin';
import { MapPosition } from '../../models/mapPosition';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: [ './map.component.scss' ],
    animations: [ fadeInOut, fadeInOutHalfOpacity ]
})
export class MapComponent implements OnInit, OnChanges, OnDestroy, AfterViewInit {
    private map: Microsoft.Maps.Map
    private htmlLayer: HtmlPushpinLayer
    private htmlClusterLayer: HtmlPushpinLayer
    private clusterLayer: Microsoft.Maps.ClusterLayer
    private clusteringStarted = false
    private addedPins: HtmlPushpin[] = []
    private addedClusterPins: HtmlPushpin[] = []
    private searchManager: Microsoft.Maps.Search.SearchManager;
    spinnerColors = spinnerColors
    mapLoaded = false
    pinsFocused = false
    selectedLocation: MapPosition;
    activeResponse: Response;

    selectingLocation = false;
    showOverlay$: Observable<boolean>
    showSelectingLocation$: Observable<boolean>;
    activeResponseSub$: Subscription;

    @Input()
    defaultOptions: MapDefaults

    @Input()
    showActivateResponse?: boolean

    @Input()
    pins: MapPin[] = []

    @Output() onLoad = new EventEmitter();

    @ViewChild('redPinSpinner')
    redPinSpinner: CircleSpinnerComponent

    @ViewChild('bluePinSpinner')
    bluePinSpinner: CircleSpinnerComponent

    @ViewChild('greenPinSpinner')
    greenPinSpinner: CircleSpinnerComponent

    @ViewChild('yellowPinSpinner')
    yellowPinSpinner: CircleSpinnerComponent

    @ViewChild('greyPinSpinner')
    greyPinSpinner: CircleSpinnerComponent

    constructor (private cdr: ChangeDetectorRef, private store: Store<AppState>) { }

    ngOnInit() {
        this.updateMap()
        this.showOverlay$ = this.store.pipe(select(selectingActionPlanSelector))
        this.showSelectingLocation$ = this.store.pipe(select(showSelectingLocationSelector));
        this.activeResponseSub$ = this.store.pipe(
            select(activeResponseSelector),
            filter(({ activeResponse }) => activeResponse !== null))
            .subscribe(({ activeResponse }) => this.activeResponse = activeResponse);
    }

    ngOnDestroy() {
        this.activeResponseSub$.unsubscribe();
    }

    ngAfterViewInit() {
        this.initMapAfterLoad()
    }

    ngOnChanges() {
        this.updateMap()
    }

    focusEvents = (events: Event[]) => {
        const pinsToFocus = this.pins.filter(
            pin =>
                pin.event &&
                events.some(event => event.eventClusterId === pin.event.eventClusterId)

        )

        this.focusPins(pinsToFocus, this.defaultOptions.zoom)
    }

    focusPins = (pins: MapPin[], zoom?: number) => {
        const locations = pins.map(pin => this.getPinLocation(pin))

        if (locations.length > 0) {
            const bounds = Microsoft.Maps.LocationRect.fromLocations(locations)

            this.setMapBounds(bounds, zoom)
            this.pinsFocused = true
        }
    }

    focusAllPins() {
        this.focusPins(this.pins)
    }

    onZoomIn() {
        const zoom = this.map.getZoom() + 1
        this.map.setView({ zoom })
    }

    onZoomOut() {
        const zoom = this.map.getZoom() - 1
        this.map.setView({ zoom })
    }

    setLocation() {
        this.selectedLocation = null;
        this.selectingLocation = true;
    }

    restartSetLocation() {
        this.selectedLocation = null;
        this.selectingLocation = false;
        this.updateResponseLocation();
    }

    hideSelectLocation() {
        this.hideShowSelectingLocation();
        this.selectedLocation = null;
        this.selectingLocation = false;
    }

    confirmLocation() {
        this.store.dispatch(new AddLocationToActiveResponse({ location: this.selectedLocation, responseId: this.activeResponse.responseId }));
        this.hideShowSelectingLocation();
        this.selectedLocation = null;
        this.selectingLocation = false;
    }

    getAddressByLocation(longitude: number, latitude: number, callback: any) {
        if (this.mapLoaded) {
            var searchRequest = {
                location: new Microsoft.Maps.Location(latitude, longitude),
                callback: function (r) {
                    if (callback) {
                        //Tell the user the name of the result.
                        callback(r.name);
                    }
                },
                errorCallback: function (e) {
                    //If there is an error, alert the user about it.
                    alert("Unable to reverse geocode location.");
                }
            };

            //Make the reverse geocode request.
            this.searchManager.reverseGeocode(searchRequest);
        } else {
            setTimeout(() => {
                this.getAddressByLocation(longitude, latitude, callback);
            }, 100)
        }
    }

    private hideShowSelectingLocation() {
        this.store.dispatch(new ShowSelectingLocation({ showSelectingLocation: false }))
    }

    private updateResponseLocation(geolocation: GeoLocation = null) {
        if (this.activeResponse) {
            this.store.dispatch(new UpdateResponse({
                response: {
                    id: this.activeResponse.responseId,
                    changes: {
                        geolocation,
                    }
                }
            }));
        }
    }

    private initMapAfterLoad = () => {
        const mapLoaded = localStorage.getItem('mapLoaded') === 'true'
        if (mapLoaded) {
            this.initMap()
        } else {
            setTimeout(this.initMapAfterLoad, 1000)
        }
    }

    private updateMap = () => {
        if (this.mapLoaded) {
            if (this.defaultOptions.useHtmlLayer) {
                const updatedPins = this.pins.filter(pin =>
                    this.addedPins.some(ap => ap.metadata.deviceId === pin.deviceId)
                )
                updatedPins.forEach(pin => {
                    const currentPin = this.addedPins.find(
                        ap => ap.metadata.deviceId === pin.deviceId
                    )
                    const occurences = pin.event ? pin.event.eventCount : 0
                    const tooltip = pin.event ? pin.event.eventType === EventType.Message ? pin.event.events[ 0 ].metadata.username : null : null;
                    currentPin.metadata = pin
                    currentPin.setOptions({
                        htmlContent: this.getHtmlElement(occurences, 1, pin.color, tooltip),
                        location: this.getPinLocation(pin),
                    })
                })

                const removedPins = this.addedPins.filter(
                    ap => !this.pins.some(p => p.deviceId === ap.metadata.deviceId)
                )

                const newPins = this.pins.filter(
                    pin =>
                        !this.addedPins.some(ap => ap.metadata.deviceId === pin.deviceId)
                )
                this.addPinsToMap(newPins)
                this.addedPins = this.addedPins.filter(
                    ap =>
                        !removedPins.some(
                            rp => rp.metadata.deviceId === ap.metadata.deviceId
                        )
                )
                removedPins.forEach(pin => {
                    this.htmlLayer.remove(pin)
                })

                const clusterPins = this.addedPins.map(pin =>
                    this.createClusterPin(pin.metadata)
                )
                this.clusterLayer.clear()
                this.clusterLayer.setPushpins(clusterPins)
            } else {
                const clusterPins = this.pins.map(pin => this.createClusterPin(pin))
                this.clusterLayer.clear()
                this.clusterLayer.setPushpins(clusterPins)
            }

            if (!this.pinsFocused && this.pins.length > 0) {
                this.focusPins(this.pins)
            }
        }
    }

    private addPinsToMap = (mapPins: MapPin[]) => {
        if (this.htmlLayer) {
            const pins = mapPins.map(pin => this.createHtmlPin(pin))
            this.addedPins.push(...pins)
            this.htmlLayer.add(pins)
        }
    }

    private initMap = () => {
        this.map = new Microsoft.Maps.Map(
            `#${this.defaultOptions.mapId || 'map'}`,
            {
                credentials: environment.bingMapsKey,
                center:
                    this.pins.length === 1 ? this.getPinLocation(this.pins[ 0 ]) : null,
                customMapStyle: environment.mapDefaults.style,
            }
        )

        // Create an infobox at the center of the map but don't show it.
        const infobox = new Microsoft.Maps.Infobox(this.map.getCenter(), {
            offset: new Microsoft.Maps.Point(0, 0),
            showCloseButton: false,
            visible: false,
        })
        infobox.setMap(this.map)

        Microsoft.Maps.registerModule(
            'HtmlPushpinLayerModule',
            '/assets/map.html-pin-layer.module.js'
        )

        this.map.setOptions({
            showLocateMeButton: false,
            showZoomButtons: false,
            showLogo: false,
            showScalebar: false,
            showTermsLink: false,
            showMapTypeSelector: false,
            disableStreetside: true,
            disableStreetsideAutoCoverage: true,
        })

        Microsoft.Maps.loadModule(
            [ 'HtmlPushpinLayerModule', 'Microsoft.Maps.Clustering', 'Microsoft.Maps.Search' ],
            () => {
                if (this.defaultOptions.useHtmlLayer) {
                    this.createHtmlPushpinLayer()
                    this.createHtmlClusterLayer()
                }
                this.createClusterLayer()

                this.searchManager = new Microsoft.Maps.Search.SearchManager(this.map);

                this.mapLoaded = true
                this.onLoad.emit(true);
                this.updateMap()
                this.initMapEvents();
                this.cdr.markForCheck()
            }
        )
    }

    private initMapEvents = () => {
        Microsoft.Maps.Events.addHandler(this.map, 'click', this.onMapClick);
    }

    private onMapClick = (event: MapClick) => {
        if (this.selectingLocation) {
            this.selectedLocation = event.location;
            this.selectingLocation = false;
            this.updateResponseLocation(event.location);
            this.cdr.markForCheck(); // all events output from bing maps do not fire angular checks
        }
    }

    private createHtmlClusterLayer = () => {
        this.htmlClusterLayer = new HtmlPushpinLayer()

        this.map.layers.insert(this.htmlClusterLayer)
    }

    private createClusterLayer = () => {
        this.clusterLayer = new Microsoft.Maps.ClusterLayer([], {
            visible: !this.defaultOptions.useHtmlLayer,
            clusteredPinCallback: this.clusteredPinCallback,
            callback: this.clusteringCompletedCallback,
            gridSize: 120,
        })

        this.map.layers.insert(this.clusterLayer)
    }

    private createHtmlPushpinLayer = () => {
        // Create an Html Pushpin Layer
        this.htmlLayer = new HtmlPushpinLayer()
        // Add the HTML pushpin to the map.
        this.map.layers.insert(this.htmlLayer)
    }

    private clusteringCompletedCallback = () => {
        if (!this.defaultOptions.useHtmlLayer) {
            return
        }

        this.clusteringStarted = false
        const displayedClusterPins = this.clusterLayer.getDisplayedPushpins()
        const pinsToShow = this.addedPins.filter(ap =>
            displayedClusterPins.some(
                p => p.metadata.deviceId === ap.metadata.deviceId
            )
        )
        pinsToShow.forEach(pin => {
            pin.setOptions({ visible: true })
        })

        const pinsToRemove = []
        this.addedClusterPins
            .filter(ap => ap.metadata !== undefined && ap.metadata !== null)
            .forEach(cp => {
                const pin = this.clusterLayer.getClusterPushpinByGridKey(
                    cp.metadata.gridKey
                )
                if (!pin) {
                    cp.metadata.containedPins.forEach(p =>
                        p.setOptions({ visible: true })
                    )
                    pinsToRemove.push(cp)
                } else {
                    cp.metadata.containedPins.forEach(p =>
                        p.setOptions({ visible: false })
                    )
                }
            })

        pinsToRemove.forEach(pin => this.htmlClusterLayer.remove(pin))
        this.addedClusterPins = this.addedClusterPins.filter(
            ap => ap.metadata !== undefined && ap.metadata !== null
        )

        this.cdr.markForCheck()
    }

    private clusteredPinCallback = (cluster: Microsoft.Maps.ClusterPushpin) => {
        if (!this.defaultOptions.useHtmlLayer) {
            return
        }

        const clusterPins = cluster.containedPushpins
        const htmlPins = this.htmlLayer.getPushpins()

        if (!this.clusteringStarted) {
            this.clusteringStarted = true
            this.htmlClusterLayer.clear()
            this.cdr.markForCheck()
        }

        // hide pins for this cluster
        const pinsToHide = htmlPins.filter(htmlPin =>
            clusterPins.some(
                cPin =>
                    (cPin.metadata as MapPin).deviceId ===
                    (htmlPin.metadata as MapPin).deviceId
            )
        )
        pinsToHide.forEach(pin => pin.setOptions({ visible: false }))

        const occurences = pinsToHide.reduce(
            (a, v) =>
                (a += v.metadata.event ? (v.metadata as MapPin).event.eventCount : 0),
            0
        )
        const htmlClusterPin = this.createHtmlClusterPin(
            cluster.getLocation(),
            occurences,
            pinsToHide,
            cluster.gridKey
        )

        this.addedClusterPins.push(htmlClusterPin)
        this.htmlClusterLayer.add(htmlClusterPin)
    }

    private setMapBounds = (bounds: Microsoft.Maps.LocationRect, zoom?: number) => {
        this.map.setView({
            bounds: bounds,
            padding: this.defaultOptions.padding || 100,
        })
        if (zoom) {
            this.map.setView({ zoom: zoom || this.defaultOptions.zoom })
        }
    }

    private getPinLocation = (pin: MapPin) => {
        return new Microsoft.Maps.Location(
            pin.geolocation.latitude,
            pin.geolocation.longitude
        )
    }

    private createHtmlPin(pin: MapPin) {
        const tooltip = pin.event ? pin.event.eventType === EventType.Message ? pin.event.events[ 0 ].metadata.username : null : null;
        const occurences = pin.event ? pin.event.eventCount : 0;
        const html = this.getHtmlElement(occurences, 1, pin.color, tooltip)

        const anchor = new Microsoft.Maps.Point(30, 30)

        const htmlPin = new HtmlPushpin(this.getPinLocation(pin), html, { anchor })
        Microsoft.Maps.Events.addHandler(htmlPin, 'click', () => { this.handlePinClick(pin.event); })
        Microsoft.Maps.Events.addHandler(htmlPin, 'dblclick', () => { this.handlePinDblClick(this.getPinLocation(pin)); })
        htmlPin.metadata = pin

        return htmlPin
    }

    private handlePinClick(event: Event) {
        if (!event) { return; }
        this.store.dispatch(new ShowEventInEventBar({ event }))
    }

    private handlePinDblClick(location: Microsoft.Maps.Location) {
        const bounds = Microsoft.Maps.LocationRect.fromLocations([ location ]);

        this.setMapBounds(bounds, this.map.getZoom() + 2);
    }

    private createClusterPin(pin: MapPin) {
        const newPin = new Microsoft.Maps.Pushpin(this.getPinLocation(pin), {
            icon: 'assets/icons/pin.svg',
            anchor: new Microsoft.Maps.Point(30, 30),
        })
        newPin.metadata = pin
        return newPin
    }

    private createHtmlClusterPin(
        location: Microsoft.Maps.Location,
        occurences: number,
        containedPins: HtmlPushpin[],
        gridKey: any
    ) {
        const colors = containedPins
            .filter(cp => cp.metadata.color !== null)
            .map(cp => cp.metadata.color)
        const color = getRankingColor(colors)

        const html = this.getHtmlElement(occurences, containedPins.length, color)
        const anchor = new Microsoft.Maps.Point(30, 30)

        const pin = new HtmlPushpin(location, html, { anchor })
        pin.metadata = {
            containedPins,
            gridKey,
        }

        Microsoft.Maps.Events.addHandler(pin, 'dblclick', () => { this.handlePinDblClick(location); })

        return pin
    }

    private getHtmlElement(occurences: number, devices: number, color?: string, specialTooltip?: string) {
        const occurencesString = `${occurences}x`;
        const tooltip = specialTooltip ? specialTooltip : devices > 1 ? `${devices} Devices` : '';
        if (occurences > 0) {
            if (color) {
                switch (color.toLowerCase()) {
                    case 'red':
                        return this.redPinSpinner.getSpinnerElement(occurencesString, tooltip, true)
                    case 'yellow':
                        return this.yellowPinSpinner.getSpinnerElement(occurencesString, tooltip, true)
                    case 'blue':
                        return this.bluePinSpinner.getSpinnerElement(occurencesString, tooltip, true)
                    case 'green':
                        return this.greenPinSpinner.getSpinnerElement(occurencesString, tooltip, true)
                    case 'grey':
                        return this.greyPinSpinner.getSpinnerElement(occurencesString, tooltip, true)
                }
            }
            return this.bluePinSpinner.getSpinnerElement(occurencesString, tooltip)
        } else {
            const displayedTooltip = tooltip.length === 0 ? '' : `<div style="height: 30px; display: flex; justify-content: center; align-items: center; position: absolute; top: -40px; width: 100%; opacity: 0.6; z-index: 3;">
                <div style="display: flex; justify-content: center; align-items: center; background-color: black; border-radius: 4px; font-size: 14px; color: white; padding: 5px 20px; white-space: nowrap;">${tooltip}</div>
                <div style="width: 0; height: 0; border-left: 10px solid transparent; border-right: 10px solid transparent; border-top: 10px solid black; position: absolute; bottom: -6px;"></div>
            </div>`
            return `
            <div>
                ${displayedTooltip}
                <img src="assets/icons/pin.svg" style="cursor: pointer" />
            </div>
            `

        }
    }
}
