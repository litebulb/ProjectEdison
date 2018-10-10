import { Component, Input, OnInit, OnDestroy } from '@angular/core'
import { MapDefaults } from '../../../map/models/mapDefaults'
import { Event } from '../../../../reducers/event/event.model'
import { AppState } from '../../../../reducers'
import { Store, select } from '@ngrx/store'
import { MapPin } from '../../../map/models/mapPin'
import {
  ShowEvents,
  SelectActiveEvent,
} from '../../../../reducers/event/event.actions'
import { spinnerColors } from '../../../../shared/spinnerColors'
import { activeEventSelector } from '../../../../reducers/event/event.selectors'
import { Subscription } from 'rxjs'
import { responsesSelector } from '../../../../reducers/response/response.selectors'
import { Response } from '../../../../reducers/response/response.model'
import { SelectActiveResponse } from '../../../../reducers/response/response.actions'

@Component({
  selector: 'app-event-card',
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.scss'],
})
export class EventCardComponent implements OnInit, OnDestroy {
  mapOptions: MapDefaults
  pins: MapPin[]
  mapVisible = false
  devices: MapPin[]
  eventEvents: any[]
  eventEventsCount: number
  circleColor = spinnerColors.activeCircleColor
  spinnerColor = spinnerColors.activeSpinnerColor
  active = false
  response: Response

  @Input()
  event: Event

  private responsesSub$: Subscription
  private activeEventSub$: Subscription

  constructor(private store: Store<AppState>) {}

  ngOnInit() {
    this.mapOptions = {
      mapId: this.event.eventClusterId,
      height: '100px',
      padding: 10,
      useHtmlLayer: false,
      zoom: 15,
    }

    this.eventEvents = this.event.events.slice(0, 3)
    this.eventEventsCount = this.event.eventCount

    this.setupPin()

    this.responsesSub$ = this.store.pipe(select(responsesSelector)).subscribe(responses => {
      this.response = responses.find(
        response => response.primaryEventClusterId === this.event.eventClusterId
      )
    })
  }

  ngOnDestroy() {
    this.activeEventSub$ && this.activeEventSub$.unsubscribe()
    this.responsesSub$.unsubscribe()
  }

  setupPin() {
    this.pins = [
      {
        ...this.event.device,
        events: [this.event],
      },
    ]
  }

  showEvent = () => {
    this.store.dispatch(new ShowEvents({ events: [this.event] }))
  }

  getBorderStyle = () => {
    if (this.active) {
      return {
        border: '1px solid #3322FF',
      }
    }

    return ''
  }

  activateResponse = () => {
    this.activeEventSub$ = this.store
      .pipe(select(activeEventSelector))
      .subscribe(activeEvent => {
        if (
          activeEvent &&
          activeEvent.eventClusterId === this.event.eventClusterId
        ) {
          this.active = true
        } else if (this.activeEventSub$) {
          this.active = false
          this.activeEventSub$.unsubscribe()
        }
      })
    this.store.dispatch(new SelectActiveEvent({ event: this.event }))
  }

  manageResponse = () => {
    this.store.dispatch(new SelectActiveResponse({ response: this.response }))
  }

  toggleMapVisibility = () => {
    this.mapVisible = !this.mapVisible
  }
}
