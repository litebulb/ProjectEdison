import { Component, OnInit, OnDestroy } from '@angular/core'
import { Event } from '../../../../reducers/event/event.model'
import { AppState } from '../../../../reducers'
import { Store, select } from '@ngrx/store'
import { eventsSelector } from 'src/app/reducers/event/event.selectors'
import { ShowEvents } from '../../../../reducers/event/event.actions'
import { spinnerColors } from '../../../../shared/spinnerColors'
import { fadeInOut } from '../../../../shared/animations/fadeInOut'
import { responsesSelector } from '../../../../reducers/response/response.selectors'
import { getRankingColor } from '../../../../shared/colorRank'
import {
  ResponseState,
  Response,
} from '../../../../reducers/response/response.model'
import { Subscription } from 'rxjs'

@Component({
  selector: 'app-event-bar',
  templateUrl: './event-bar.component.html',
  styleUrls: ['./event-bar.component.scss'],
  animations: [fadeInOut],
})
export class EventBarComponent implements OnInit, OnDestroy {
  allEvents: Event[]
  events: Event[]
  recentEvents: Event[]
  recentEventsLazy: Event[]
  responses: Response[] = []
  eventCount: number
  lazyCount = 3
  circleColor: string
  spinnerColor: string
  animate = true

  private eventsSub$: Subscription
  private responsesSub$: Subscription

  constructor(private store: Store<AppState>) {}

  ngOnInit() {
    this.eventsSub$ = this.store
      .pipe(select(eventsSelector))
      .subscribe(events => {
        this.onEventsUpdate(events)
      })

    this.responsesSub$ = this.store
      .pipe(select(responsesSelector))
      .subscribe(responses => {
        this.responses = responses
        this.updateSpinner()
      })
  }

  ngOnDestroy() {
    this.eventsSub$.unsubscribe()
    this.responsesSub$.unsubscribe()
  }

  onEventsUpdate = (events: Event[]) => {
    this.allEvents = events
    this.events = events.filter((v, i) => i < 1)
    this.recentEvents = events.filter((v, i) => i > 0)
    this.recentEventsLazy = events.filter((v, i) => i > 0 && i < this.lazyCount)

    this.eventCount = this.getEventCount()
    this.animate = this.eventCount > 0

    this.updateSpinner()
  }

  getEventCount() {
    return this.allEvents
      .filter(e => e.closureDate === null)
      .reduce((a, v) => (a += v.eventCount), 0)
  }

  updateSpinner = () => {
    if (this.responses.length <= 0) {
      this.setActiveColors()
    }

    const activeResponses = this.responses
      .filter(response => response.event)
      .filter(
        response =>
          this.events.some(
            event => event.eventClusterId === response.event.eventClusterId
          ) ||
          this.recentEvents.some(
            event => event.eventClusterId === response.event.eventClusterId
          )
      )

    const activeColors = activeResponses.map(
      ar =>
        ar.responseState === ResponseState.Active
          ? ar.actionPlan.color.toLowerCase()
          : 'green'
    )

    const eventCount = this.getEventCount()
    if (activeColors.length > 0) {
      if (activeColors.length !== eventCount) {
        activeColors.push('blue')
      }
      const activeColor = getRankingColor(activeColors)
      this.updateSpinnerColors(activeColor)
    } else {
      this.setActiveColors()
    }
  }

  updateSpinnerColors = (color: string) => {
    switch (color) {
      case 'red':
        this.circleColor = spinnerColors.redCircleColor
        this.spinnerColor = spinnerColors.redSpinnerColor
        break
      case 'yellow':
        this.circleColor = spinnerColors.yellowCircleColor
        this.spinnerColor = spinnerColors.yellowSpinnerColor
        break
      case 'blue':
        this.circleColor = spinnerColors.blueCircleColor
        this.spinnerColor = spinnerColors.blueSpinnerColor
        break
      case 'green':
        this.circleColor = spinnerColors.greenCircleColor
        this.spinnerColor = spinnerColors.greenSpinnerColor
        break
    }
  }

  setActiveColors = () => {
    if (this.eventCount > 0) {
      this.circleColor = spinnerColors.activeCircleColor
      this.spinnerColor = spinnerColors.activeSpinnerColor
    } else {
      this.circleColor = spinnerColors.inactiveCircleColor
      this.spinnerColor = spinnerColors.inactiveSpinnerColor
    }
  }

  onScroll = () => {
    if (this.recentEvents.length !== this.recentEventsLazy.length) {
      const startSlice = this.recentEventsLazy.length - 1
      const endSlice = this.recentEventsLazy.length + this.lazyCount - 1
      const slice = this.recentEvents.slice(startSlice, endSlice)

      this.recentEventsLazy.push(...slice)
    }
  }

  showEvents = () => {
    this.store.dispatch(
      new ShowEvents({ events: [...this.events, ...this.recentEvents] })
    )
  }
}
