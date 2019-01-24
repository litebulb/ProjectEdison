import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
} from '@angular/core';

import { environment } from '../../../../../environments/environment';
import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { fadeInOutHalfOpacity } from '../../../../core/animations/fadeInOutHalfOpacity';
import { getRankingColor } from '../../../../core/colorRank';
import { GeoLocation } from '../../../../core/models/geoLocation';
import { spinnerColors } from '../../../../core/spinnerColors';
import { Device } from '../../../../reducers/device/device.model';
import { Event, EventType } from '../../../../reducers/event/event.model';
import { Response } from '../../../../reducers/response/response.model';
import { MapClick } from '../../models/mapClick';
import { MapDefaults } from '../../models/mapDefaults';
import { MapPin } from '../../models/mapPin';
import { MapPosition } from '../../models/mapPosition';
import { Subscription, interval } from 'rxjs';
import { ISpinnerIcon, SpinnerIcon } from '../../helpers/spinner-icon-builder';
import { ImageIcon } from '../../helpers/image-icon-builder';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss'],
  animations: [fadeInOut, fadeInOutHalfOpacity],
})
export class MapComponent implements OnInit, OnChanges, AfterViewInit {
  private _map: Microsoft.Maps.Map;
  private _htmlLayer: HtmlPushpinLayer;
  private _htmlClusterLayer: HtmlPushpinLayer;
  private _clusterLayer: Microsoft.Maps.ClusterLayer;
  private _clusteringStarted = false;
  private _addedPins: HtmlPushpin[] = [];
  private _addedClusterPins: HtmlPushpin[] = [];
  private _searchManager: Microsoft.Maps.Search.SearchManager;
  private _mapLoaded$: Subscription;
  private _pinsFocused = false;

  mapLoaded = false;
  selectedLocation: MapPosition;
  selectingLocation = false;
  showOverlay: boolean;
  showSelectingLocation: boolean;

  @Input() activeResponse: Response;
  @Input() defaultOptions: MapDefaults;
  @Input() showActivateResponse?: boolean;
  @Input() pins: MapPin[] = [];

  @Output() onLoad = new EventEmitter();
  @Output() onAddLocationToResponse = new EventEmitter();
  @Output() onUpdateResponseLocation = new EventEmitter();
  @Output() onEventClick = new EventEmitter();

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this._updateMap();
  }

  ngAfterViewInit() {
    this._initMapAfterLoad();
  }

  ngOnChanges() {
    this._updateMap();
  }

  toggleOverlay(state?: boolean) {
    if (state !== undefined) {
      this.showOverlay = state;
    } else {
      this.showOverlay = !this.showOverlay;
    }
  }

  toggleSelectResponseLocation(state?: boolean) {
    if (state !== undefined) {
      this.showSelectingLocation = state;
    } else {
      this.showSelectingLocation = !this.showSelectingLocation;
    }
  }

  focusDevices = (devices: Device[]) => {
    const pinsToFocus = this.pins.filter(pin =>
      devices.some(event => event.deviceId === pin.deviceId)
    );

    this.focusPins(pinsToFocus, this.defaultOptions.zoom);
  };

  focusEvents = (events: Event[]) => {
    console.log(events);
    const pinsToFocus = this.pins.filter(
      pin =>
        pin.event &&
        events.some(event => event.eventClusterId === pin.event.eventClusterId)
    );
    console.log(pinsToFocus);
    this.focusPins(pinsToFocus, this.defaultOptions.zoom);
  };

  focusPins = (pins: MapPin[], zoom?: number) => {
    const locations = pins.map(pin => this._getPinLocation(pin));

    if (locations.length > 0) {
      const bounds = Microsoft.Maps.LocationRect.fromLocations(locations);

      this._setMapBounds(bounds, zoom);
      this._pinsFocused = true;
    }
  };

  focusAllPins() {
    this.focusPins(this.pins);
  }

  getAddressByLocation(longitude: number, latitude: number, callback: any) {
    if (this.mapLoaded) {
      var searchRequest = {
        location: new Microsoft.Maps.Location(latitude, longitude),
        callback: function(r) {
          if (callback) {
            //Tell the user the name of the result.
            callback(r.name);
          }
        },
        errorCallback: function(e) {
          //If there is an error, alert the user about it.
          alert('Unable to reverse geocode location.');
        },
      };

      //Make the reverse geocode request.
      this._searchManager.reverseGeocode(searchRequest);
    } else {
      setTimeout(() => {
        this.getAddressByLocation(longitude, latitude, callback);
      }, 100);
    }
  }

  protected onZoomIn() {
    const zoom = this._map.getZoom() + 1;
    this._map.setView({ zoom });
  }

  protected onZoomOut() {
    const zoom = this._map.getZoom() - 1;
    this._map.setView({ zoom });
  }

  protected setLocation() {
    this.selectedLocation = null;
    this.selectingLocation = true;
  }

  protected restartSetLocation() {
    this.selectedLocation = null;
    this.selectingLocation = false;
    this._updateResponseLocation();
  }

  protected hideSelectLocation() {
    this._hideSelectResponseLocation();
    this.selectedLocation = null;
    this.selectingLocation = false;
  }

  protected confirmLocation() {
    this.onAddLocationToResponse.emit({
      location: this.selectedLocation,
      responseId: this.activeResponse.responseId,
    });
    this._hideSelectResponseLocation();
    this.selectedLocation = null;
    this.selectingLocation = false;
  }

  private _hideSelectResponseLocation() {
    this.toggleSelectResponseLocation(false);
  }

  private _updateResponseLocation(geolocation: GeoLocation = null) {
    if (this.activeResponse) {
      this.onUpdateResponseLocation.emit({
        geolocation,
        responseId: this.activeResponse.responseId,
      });
    }
  }

  private _initMapAfterLoad = () => {
    const mapLoaded = localStorage.getItem('mapLoaded') === 'true';
    if (mapLoaded) {
      this._initMap();
    } else {
      setTimeout(this._initMapAfterLoad, 1000);
    }
  };

  private _updateMap = () => {
    if (this.mapLoaded) {
      if (this._mapLoaded$) {
        this._mapLoaded$.unsubscribe();
      }
      if (this.defaultOptions.useHtmlLayer) {
        const updatedPins = this.pins.filter(pin =>
          this._addedPins.some(ap => ap.metadata.deviceId === pin.deviceId)
        );
        updatedPins.forEach(pin => {
          const currentPin = this._addedPins.find(
            ap => ap.metadata.deviceId === pin.deviceId
          );
          const occurences = pin.event ? pin.event.eventCount : 0;
          const tooltip = pin.event
            ? pin.event.eventType === EventType.Message
              ? pin.event.events[0].metadata.username
              : null
            : null;
          currentPin.metadata = pin;
          currentPin.setOptions({
            htmlContent: this._getHtmlElement(
              occurences,
              1,
              pin.color,
              tooltip,
              pin.icon
            ),
            location: this._getPinLocation(pin),
          });
        });

        const removedPins = this._addedPins.filter(
          ap => !this.pins.some(p => p.deviceId === ap.metadata.deviceId)
        );

        const newPins = this.pins.filter(
          pin =>
            !this._addedPins.some(ap => ap.metadata.deviceId === pin.deviceId)
        );
        this._addPinsToMap(newPins);
        this._addedPins = this._addedPins.filter(
          ap =>
            !removedPins.some(
              rp => rp.metadata.deviceId === ap.metadata.deviceId
            )
        );
        removedPins.forEach(pin => {
          this._htmlLayer.remove(pin);
        });

        const clusterPins = this._addedPins.map(pin =>
          this._createClusterPin(pin.metadata)
        );
        this._clusterLayer.clear();
        this._clusterLayer.setPushpins(clusterPins);
      } else {
        const clusterPins = this.pins.map(pin => this._createClusterPin(pin));
        this._clusterLayer.clear();
        this._clusterLayer.setPushpins(clusterPins);
      }

      if (!this._pinsFocused && this.pins.length > 0) {
        this.focusPins(this.pins);
      }
    } else if (this._mapLoaded$ === undefined || this._mapLoaded$.closed) {
      // make sure the map gets updated on load
      // the onload function that gets fired from ms maps is not reliable
      this._mapLoaded$ = interval(1000).subscribe(() => {
        if (this.mapLoaded) {
          this._updateMap();
        }
      });
    }
  };

  private _addPinsToMap = (mapPins: MapPin[]) => {
    if (this._htmlLayer) {
      const pins = mapPins.map(pin => this._createHtmlPin(pin));
      this._addedPins.push(...pins);
      this._htmlLayer.add(pins);
    }
  };

  private _initMap = () => {
    this._map = new Microsoft.Maps.Map(
      `#${this.defaultOptions.mapId || 'map'}`,
      {
        credentials: environment.bingMapsKey,
        center:
          this.pins.length === 1 ? this._getPinLocation(this.pins[0]) : null,
        customMapStyle: environment.mapDefaults.style,
      }
    );

    // Create an infobox at the center of the map but don't show it.
    const infobox = new Microsoft.Maps.Infobox(this._map.getCenter(), {
      offset: new Microsoft.Maps.Point(0, 0),
      showCloseButton: false,
      visible: false,
    });
    infobox.setMap(this._map);

    Microsoft.Maps.registerModule(
      'HtmlPushpinLayerModule',
      '/assets/map.html-pin-layer.module.js'
    );

    this._map.setOptions({
      showLocateMeButton: false,
      showZoomButtons: false,
      showLogo: false,
      showScalebar: false,
      showTermsLink: false,
      showMapTypeSelector: false,
      disableStreetside: true,
      disableStreetsideAutoCoverage: true,
    });

    Microsoft.Maps.loadModule(
      [
        'HtmlPushpinLayerModule',
        'Microsoft.Maps.Clustering',
        'Microsoft.Maps.Search',
      ],
      () => {
        if (this.defaultOptions.useHtmlLayer) {
          this._createHtmlPushpinLayer();
          this._createHtmlClusterLayer();
        }
        this._createClusterLayer();

        this._searchManager = new Microsoft.Maps.Search.SearchManager(
          this._map
        );

        this.mapLoaded = true;
        this.onLoad.emit(true);
        this._updateMap();
        this._initMapEvents();
        this.cdr.markForCheck();
      }
    );
  };

  private _initMapEvents = () => {
    Microsoft.Maps.Events.addHandler(this._map, 'click', this._onMapClick);
  };

  private _onMapClick = (event: MapClick) => {
    if (this.selectingLocation) {
      this.selectedLocation = event.location;
      this.selectingLocation = false;
      this._updateResponseLocation(event.location);
      this.cdr.markForCheck(); // all events output from bing maps do not fire angular checks
    }
  };

  private _createHtmlClusterLayer = () => {
    this._htmlClusterLayer = new HtmlPushpinLayer();

    this._map.layers.insert(this._htmlClusterLayer);
  };

  private _createClusterLayer = () => {
    this._clusterLayer = new Microsoft.Maps.ClusterLayer([], {
      visible: !this.defaultOptions.useHtmlLayer,
      clusteredPinCallback: this._clusteredPinCallback,
      callback: this._clusteringCompletedCallback,
      gridSize: 120,
    });

    this._map.layers.insert(this._clusterLayer);
  };

  private _createHtmlPushpinLayer = () => {
    // Create an Html Pushpin Layer
    this._htmlLayer = new HtmlPushpinLayer();
    // Add the HTML pushpin to the map.
    this._map.layers.insert(this._htmlLayer);
  };

  private _clusteringCompletedCallback = () => {
    if (!this.defaultOptions.useHtmlLayer) {
      return;
    }

    this._clusteringStarted = false;
    const displayedClusterPins = this._clusterLayer.getDisplayedPushpins();
    const pinsToShow = this._addedPins.filter(ap =>
      displayedClusterPins.some(
        p => p.metadata.deviceId === ap.metadata.deviceId
      )
    );
    pinsToShow.forEach(pin => {
      pin.setOptions({ visible: true });
    });

    const pinsToRemove = [];
    this._addedClusterPins
      .filter(ap => ap.metadata !== undefined && ap.metadata !== null)
      .forEach(cp => {
        const pin = this._clusterLayer.getClusterPushpinByGridKey(
          cp.metadata.gridKey
        );
        if (!pin) {
          cp.metadata.containedPins.forEach(p =>
            p.setOptions({ visible: true })
          );
          pinsToRemove.push(cp);
        } else {
          cp.metadata.containedPins.forEach(p =>
            p.setOptions({ visible: false })
          );
        }
      });

    pinsToRemove.forEach(pin => this._htmlClusterLayer.remove(pin));
    this._addedClusterPins = this._addedClusterPins.filter(
      ap => ap.metadata !== undefined && ap.metadata !== null
    );

    this.cdr.markForCheck();
  };

  private _clusteredPinCallback = (cluster: Microsoft.Maps.ClusterPushpin) => {
    if (!this.defaultOptions.useHtmlLayer) {
      return;
    }

    const clusterPins = cluster.containedPushpins;
    const htmlPins = this._htmlLayer.getPushpins();

    if (!this._clusteringStarted) {
      this._clusteringStarted = true;
      this._htmlClusterLayer.clear();
      this.cdr.markForCheck();
    }

    // hide pins for this cluster
    const pinsToHide = htmlPins.filter(htmlPin =>
      clusterPins.some(
        cPin =>
          (cPin.metadata as MapPin).deviceId ===
          (htmlPin.metadata as MapPin).deviceId
      )
    );
    pinsToHide.forEach(pin => pin.setOptions({ visible: false }));

    const occurences = pinsToHide.reduce(
      (a, v) =>
        (a += v.metadata.event ? (v.metadata as MapPin).event.eventCount : 0),
      0
    );
    const htmlClusterPin = this._createHtmlClusterPin(
      cluster.getLocation(),
      occurences,
      pinsToHide,
      cluster.gridKey
    );

    this._addedClusterPins.push(htmlClusterPin);
    this._htmlClusterLayer.add(htmlClusterPin);
  };

  private _setMapBounds = (
    bounds: Microsoft.Maps.LocationRect,
    zoom?: number
  ) => {
    this._map.setView({
      bounds: bounds,
      padding: this.defaultOptions.padding || 100,
    });
    if (zoom) {
      this._map.setView({ zoom: zoom || this.defaultOptions.zoom });
    }
  };

  private _getPinLocation = (pin: MapPin) => {
    return new Microsoft.Maps.Location(
      pin.geolocation.latitude,
      pin.geolocation.longitude
    );
  };

  private _createHtmlPin(pin: MapPin) {
    const tooltip = pin.event
      ? pin.event.eventType === EventType.Message
        ? pin.event.events[0].metadata.username
        : null
      : null;
    const occurences = pin.event ? pin.event.eventCount : 0;
    const html = this._getHtmlElement(
      occurences,
      1,
      pin.color,
      tooltip,
      pin.icon
    );

    const anchor = new Microsoft.Maps.Point(30, 30);

    const htmlPin = new HtmlPushpin(this._getPinLocation(pin), html, {
      anchor,
    });
    Microsoft.Maps.Events.addHandler(htmlPin, 'click', () => {
      this._handlePinClick(pin.event);
    });
    Microsoft.Maps.Events.addHandler(htmlPin, 'dblclick', () => {
      this._handlePinDblClick(this._getPinLocation(pin));
    });
    htmlPin.metadata = pin;

    return htmlPin;
  }

  private _handlePinClick(event: Event) {
    if (!event) {
      return;
    }
    this.onEventClick.emit(event);
  }

  private _handlePinDblClick(location: Microsoft.Maps.Location) {
    const bounds = Microsoft.Maps.LocationRect.fromLocations([location]);

    this._setMapBounds(bounds, this._map.getZoom() + 2);
  }

  private _createClusterPin(pin: MapPin) {
    const newPin = new Microsoft.Maps.Pushpin(this._getPinLocation(pin), {
      icon: 'assets/icons/pin.svg',
      anchor: new Microsoft.Maps.Point(30, 30),
    });
    newPin.metadata = pin;
    return newPin;
  }

  private _createHtmlClusterPin(
    location: Microsoft.Maps.Location,
    occurences: number,
    containedPins: HtmlPushpin[],
    gridKey: any
  ) {
    const colors = containedPins
      .filter(cp => cp.metadata.color !== null)
      .map(cp => cp.metadata.color);
    const color = getRankingColor(colors);

    const html = this._getHtmlElement(occurences, containedPins.length, color);
    const anchor = new Microsoft.Maps.Point(30, 30);

    const pin = new HtmlPushpin(location, html, { anchor });
    pin.metadata = {
      containedPins,
      gridKey,
    };

    Microsoft.Maps.Events.addHandler(pin, 'dblclick', () => {
      this._handlePinDblClick(location);
    });

    return pin;
  }

  private _getHtmlElement(
    occurences: number,
    devices: number,
    color?: string,
    specialTooltip?: string,
    icon?: string
  ) {
    const occurencesString = `${occurences}x`;
    const tooltip = specialTooltip
      ? specialTooltip
      : devices > 1
      ? `${devices} Devices`
      : '';
    if (occurences > 0) {
      // default to blue spinner icon
      const options: ISpinnerIcon = {
        size: 54,
        depth: 6,
        circleContent: occurencesString,
        tooltip,
        spinnerColorRgb: { red: 0, green: 80, blue: 179 },
        circleColor: spinnerColors.blueCircleColor,
        spinnerStyle: 'cursor: pointer;',
      };
      if (color) {
        switch (color.toLowerCase()) {
          case 'red':
            options.circleColor = spinnerColors.redCircleColor;
            options.spinnerColorRgb = { red: 230, green: 44, blue: 30 };
            break;
          case 'yellow':
            options.circleColor = spinnerColors.yellowCircleColor;
            options.spinnerColorRgb = { red: 255, green: 159, blue: 33 };
            break;
          case 'green':
            options.circleColor = spinnerColors.greenCircleColor;
            options.spinnerColorRgb = { red: 0, green: 229, blue: 54 };
            break;
          case 'grey':
            options.circleColor = spinnerColors.greyCircleColor;
            options.spinnerColorRgb = { red: 128, green: 128, blue: 128 };
            break;
        }
      }
      return SpinnerIcon(options);
    } else {
      let backgroundColor = '';
      if (color) {
        switch (color.toLowerCase()) {
          case 'blue':
            backgroundColor = '#3A82FE';
            break;
          case 'green':
            backgroundColor = '#00E536';
            break;
          case 'red':
            backgroundColor = '#FA4035';
            break;
          case 'yellow':
            backgroundColor = '#FABF0D';
            break;
        }
      }
      return ImageIcon({ icon, backgroundColor, tooltip });
    }
  }
}
