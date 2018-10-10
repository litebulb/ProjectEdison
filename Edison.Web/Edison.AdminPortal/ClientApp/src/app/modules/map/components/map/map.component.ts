import {
  Component,
  OnInit,
  OnChanges,
  Input,
  AfterViewInit,
  ViewChild,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from '@angular/core';
import { MapDefaults } from '../../models/mapDefaults';
import { MapPin } from '../../models/mapPin';
import { CircleSpinnerComponent } from '../../../shared/components/circle-spinner/circle-spinner.component';
import { fadeInOut } from '../../../../shared/animations/fadeInOut';
import { environment } from '../../../../../environments/environment';
import { spinnerColors } from '../../../../shared/spinnerColors';
import { Event } from '../../../../reducers/event/event.model';
import { getRankingColor } from '../../../../shared/colorRank';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss'],
  animations: [fadeInOut],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MapComponent implements OnInit, OnChanges, AfterViewInit {
  private map: Microsoft.Maps.Map;
  private htmlLayer: HtmlPushpinLayer;
  private htmlClusterLayer: HtmlPushpinLayer;
  private clusterLayer: Microsoft.Maps.ClusterLayer;
  private clusteringStarted = false;
  private addedPins: HtmlPushpin[] = [];
  private addedClusterPins: HtmlPushpin[] = [];
  spinnerColors = spinnerColors;
  mapLoaded = false;
  pinsFocused = false;

  @Input()
  defaultOptions: MapDefaults;

  @Input()
  showActivateResponse?: boolean;

  @Input()
  pins: MapPin[] = [];

  @ViewChild('redPinSpinner')
  redPinSpinner: CircleSpinnerComponent;

  @ViewChild('bluePinSpinner')
  bluePinSpinner: CircleSpinnerComponent;

  @ViewChild('greenPinSpinner')
  greenPinSpinner: CircleSpinnerComponent;

  @ViewChild('yellowPinSpinner')
  yellowPinSpinner: CircleSpinnerComponent;

  @ViewChild('greyPinSpinner')
  greyPinSpinner: CircleSpinnerComponent;

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.updateMap();
  }

  ngAfterViewInit() {
    this.initMapAfterLoad();
  }

  ngOnChanges() {
    this.updateMap();
  }

  focusEvents = (events: Event[]) => {
    const pinsToFocus = this.pins.filter(pin =>
      pin.events.some(pEvent => events.some(event => event.eventClusterId === pEvent.eventClusterId)));

      this.focusPins(pinsToFocus);
  }

  focusPins = (pins: MapPin[]) => {
    const locations = pins.map(pin => this.getPinLocation(pin));

    if (locations.length > 0) {
      const bounds = Microsoft.Maps.LocationRect.fromLocations(locations);

      this.setMapBounds(bounds);
      this.pinsFocused = true;
    }
  }

  private initMapAfterLoad = () => {
    const mapLoaded = localStorage.getItem('mapLoaded') === 'true';
    if (mapLoaded) {
      this.initMap();
    } else {
      setTimeout(this.initMapAfterLoad, 1000);
    }
  }

  private updateMap = () => {
    if (this.mapLoaded) {
      if (this.defaultOptions.useHtmlLayer) {
        const newPins = this.pins.filter(pin => !this.addedPins.some(ap => ap.metadata.deviceId === pin.deviceId));
        this.addPinsToMap(newPins);

        const updatedPins = this.pins.filter(pin => this.addedPins.some(ap => ap.metadata.deviceId === pin.deviceId));
        updatedPins.forEach(pin => {
          const currentPin = this.addedPins.find(ap => ap.metadata.deviceId === pin.deviceId);
          const occurences = pin.events ? pin.events.reduce((a, v) => a += v.eventCount, 0) : 0;
          currentPin.metadata = pin;
          currentPin.setOptions({ htmlContent: this.getHtmlElement(occurences, pin.color) });
        });

        const removedPins = this.addedPins.filter(ap => !this.pins.some(p => p.deviceId === ap.metadata.deviceId));
        this.addedPins = this.addedPins.filter(ap => !removedPins.some(rp => rp.metadata.deviceId === ap.metadata.deviceId));
        removedPins.forEach(pin => {
          this.htmlLayer.remove(pin);
        });

        const clusterPins = this.addedPins.map(pin => this.createClusterPin(pin.metadata));
        this.clusterLayer.clear();
        this.clusterLayer.setPushpins(clusterPins);
      } else {
        const clusterPins = this.pins.map(pin => this.createClusterPin(pin));
        this.clusterLayer.clear();
        this.clusterLayer.setPushpins(clusterPins);
      }

      if (!this.pinsFocused && this.pins.length > 0) {
        this.focusPins(this.pins);
      }
    }
  }

  private addPinsToMap = (mapPins: MapPin[]) => {
    if (this.htmlLayer) {
      const pins = mapPins.map(pin => this.createHtmlPin(pin));
      this.addedPins.push(...pins);
      this.htmlLayer.add(pins);
    }
  }

  private initMap = () => {
    this.map = new Microsoft.Maps.Map(
      `#${this.defaultOptions.mapId || 'map'}`,
      {
        credentials: environment.bingMapsKey,
        center: this.pins.length === 1 ? this.getPinLocation(this.pins[0]) : null,
      },
    );

    // Create an infobox at the center of the map but don't show it.
    const infobox = new Microsoft.Maps.Infobox(this.map.getCenter(), {
      offset: new Microsoft.Maps.Point(0, 0),
      showCloseButton: false,
      visible: false,
    });
    infobox.setMap(this.map);

    Microsoft.Maps.registerModule(
      'HtmlPushpinLayerModule',
      '/assets/map.html-pin-layer.module.js'
    );

    this.map.setOptions({
      showLocateMeButton: false,
      showZoomButtons: false,
      showLogo: false,
      showScalebar: false,
      showTermsLink: false,
      showMapTypeSelector: false,
      disableStreetside: true,
      disableStreetsideAutoCoverage: true,
    });

    Microsoft.Maps.loadModule(['HtmlPushpinLayerModule', 'Microsoft.Maps.Clustering'], () => {
      if (this.defaultOptions.useHtmlLayer) {
        this.createHtmlPushpinLayer();
        this.createHtmlClusterLayer();
      }
      this.createClusterLayer();

      this.mapLoaded = true;
      this.updateMap();
      this.cdr.markForCheck();
    });
  }

  private createHtmlClusterLayer = () => {
    this.htmlClusterLayer = new HtmlPushpinLayer();

    this.map.layers.insert(this.htmlClusterLayer);
  }

  private createClusterLayer = () => {
    this.clusterLayer = new Microsoft.Maps.ClusterLayer([], {
      visible: !this.defaultOptions.useHtmlLayer,
      clusteredPinCallback: this.clusteredPinCallback,
      callback: this.clusteringCompletedCallback,
      gridSize: 120
    });

    this.map.layers.insert(this.clusterLayer);
  }

  private createHtmlPushpinLayer = () => {
    // Create an Html Pushpin Layer
    this.htmlLayer = new HtmlPushpinLayer();
    // Add the HTML pushpin to the map.
    this.map.layers.insert(this.htmlLayer);
  }

  private clusteringCompletedCallback = () => {
    if (!this.defaultOptions.useHtmlLayer) { return; }

    this.clusteringStarted = false;
    const displayedClusterPins = this.clusterLayer.getDisplayedPushpins();
    const pinsToShow = this.addedPins.filter(ap => displayedClusterPins.some(p => p.metadata.deviceId === ap.metadata.deviceId));
    pinsToShow.forEach(pin => { pin.setOptions({ visible: true}); });

    const pinsToRemove = [];
    this.addedClusterPins.filter(ap => ap.metadata !== undefined && ap.metadata !== null).forEach(cp => {
      const pin = this.clusterLayer.getClusterPushpinByGridKey(cp.metadata.gridKey);
      if (!pin) {
        cp.metadata.containedPins.forEach(p => p.setOptions({ visible: true }));
        pinsToRemove.push(cp);
      } else {
        cp.metadata.containedPins.forEach(p => p.setOptions({ visible: false }));
      }
    });

    pinsToRemove.forEach(pin => this.htmlClusterLayer.remove(pin));
    this.addedClusterPins = this.addedClusterPins.filter(ap => ap.metadata !== undefined && ap.metadata !== null);

    this.cdr.markForCheck();
  }

  private clusteredPinCallback = (cluster: Microsoft.Maps.ClusterPushpin) => {
    if (!this.defaultOptions.useHtmlLayer) { return; }

    const clusterPins = cluster.containedPushpins;
    const htmlPins = this.htmlLayer.getPushpins();

    if (!this.clusteringStarted) {
      this.clusteringStarted = true;
      this.htmlClusterLayer.clear();
      this.cdr.markForCheck();
    }

    // hide pins for this cluster
    const pinsToHide = htmlPins.filter(htmlPin =>
      clusterPins.some(cPin => (cPin.metadata as MapPin).deviceId === (htmlPin.metadata as MapPin).deviceId));
    pinsToHide.forEach(pin => pin.setOptions({ visible: false }));

    const occurences = pinsToHide.reduce((a, v) => a += v.metadata.events ?
      (v.metadata as MapPin).events.reduce((aa, vv) => aa += vv.eventCount, 0) : 0, 0);
    const htmlClusterPin = this.createHtmlClusterPin(cluster.getLocation(), occurences, pinsToHide, cluster.gridKey);

    this.addedClusterPins.push(htmlClusterPin);
    this.htmlClusterLayer.add(htmlClusterPin);
  }

  private setMapBounds = (bounds: Microsoft.Maps.LocationRect) => {
    this.map.setView({ bounds: bounds, padding: this.defaultOptions.padding || 100});
    this.map.setView({ zoom: this.defaultOptions.zoom || 14 });
  }

  private getPinLocation = (pin: MapPin) => {
    return new Microsoft.Maps.Location(pin.geolocation.latitude, pin.geolocation.longitude);
  }

  private createHtmlPin(pin: MapPin) {
    const occurences = pin.events ? pin.events.reduce((a, v) => a += v.events.length, 0) : 0;
    const html = this.getHtmlElement(occurences, pin.color);

    const anchor = new Microsoft.Maps.Point(30, 30);

    const htmlPin = new HtmlPushpin(this.getPinLocation(pin), html, { anchor });
    htmlPin.metadata = pin;

    return htmlPin;
  }

  private createClusterPin(pin: MapPin) {
    const newPin = new Microsoft.Maps.Pushpin(this.getPinLocation(pin), {
      icon: 'assets/icons/pin.svg',
      anchor: new Microsoft.Maps.Point(30, 30)
    });
    newPin.metadata = pin;
    return newPin;
  }

  private createHtmlClusterPin(location: Microsoft.Maps.Location, occurences: number, containedPins: HtmlPushpin[], gridKey: any) {
    const colors = containedPins.filter(cp => cp.metadata.color !== null).map(cp => cp.metadata.color);
    const color = getRankingColor(colors);

    const html = this.getHtmlElement(occurences, color);
    const anchor = new Microsoft.Maps.Point(30, 30);

    const pin = new HtmlPushpin(location, html, { anchor });
    pin.metadata = {
      containedPins,
      gridKey,
    };

    return pin;
  }

  private getHtmlElement(occurences: number, color?: string) {
    if (occurences > 0) {
      const occurencesString = `${occurences}x`;
      if (color) {
        switch (color.toLowerCase()) {
          case 'red':
            return this.redPinSpinner.getSpinnerElement(occurencesString);
          case 'yellow':
            return this.yellowPinSpinner.getSpinnerElement(occurencesString);
          case 'blue':
            return this.bluePinSpinner.getSpinnerElement(occurencesString);
          case 'green':
            return this.greenPinSpinner.getSpinnerElement(occurencesString);
          case 'grey':
            return this.greyPinSpinner.getSpinnerElement(occurencesString);
        }
      }
      return this.bluePinSpinner.getSpinnerElement(occurencesString);
    } else {
      return '<img src="assets/icons/pin.svg" />';
    }
  }
}
