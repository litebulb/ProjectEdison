import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core'
import { Event } from '../../../../reducers/event/event.model'
import { AppState } from '../../../../reducers'
import { Store, select } from '@ngrx/store'
import { eventsSelector } from 'src/app/reducers/event/event.selectors'
import { ShowEvents, EventActionTypes, ShowEventInEventBar } from '../../../../reducers/event/event.actions'
import { spinnerColors } from '../../../../core/spinnerColors'
import { fadeInOut } from '../../../../core/animations/fadeInOut'
import { responsesSelector } from '../../../../reducers/response/response.selectors'
import { getRankingColor } from '../../../../core/colorRank'
import {
    ResponseState,
    Response,
} from '../../../../reducers/response/response.model'
import { Subscription } from 'rxjs'
import { listFadeInOut } from '../../../../core/animations/listFadeInOut'
import { PerfectScrollbarConfigInterface, PerfectScrollbarComponent } from 'ngx-perfect-scrollbar';
import { Actions, ofType } from '@ngrx/effects';
import { SetPageData } from '../../../../reducers/app/app.actions';

@Component({
    selector: 'app-event-bar',
    templateUrl: './event-bar.component.html',
    styleUrls: [ './event-bar.component.scss' ],
    animations: [ fadeInOut, listFadeInOut ],
})
export class EventBarComponent implements OnInit, OnDestroy {
    @ViewChild(PerfectScrollbarComponent) perfectScrollbar: PerfectScrollbarComponent;

    events: Event[]
    recentEventsLazy: Event[]
    responses: Response[] = []
    eventCount: number
    lazyCount = 3
    circleColor: string
    spinnerColor: string
    animate = true
    scrollConfig: PerfectScrollbarConfigInterface = {

    }

    private eventsSub$: Subscription
    private responsesSub$: Subscription
    private showEventInEventBarSub$: Subscription;

    constructor (private store: Store<AppState>, private actions$: Actions) { }

    ngOnInit() {
        this.store.dispatch(new SetPageData({ title: 'RIGHT NOW', sidebar: true }));

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

        this.showEventInEventBarSub$ = this.actions$
            .pipe(ofType(EventActionTypes.ShowEventInEventBar))
            .subscribe(({ payload: { event } }: ShowEventInEventBar) => this.scrollToEvent(event.eventClusterId))
    }

    ngOnDestroy() {
        this.eventsSub$.unsubscribe()
        this.responsesSub$.unsubscribe()
        this.showEventInEventBarSub$.unsubscribe();
    }

    onEventsUpdate = (events: Event[]) => {
        this.events = events
        this.recentEventsLazy = events.filter((v, i) => i < this.lazyCount)

        this.eventCount = this.getEventCount()
        this.animate = this.eventCount > 0

        this.updateSpinner()
    }

    getEventCount() {
        return this.events
            .filter(e => e.closureDate === null)
            .reduce((a, v) => (a += v.eventCount), 0)
    }

    updateSpinner = () => {
        if (this.responses.length <= 0) {
            this.setActiveColors()
        }

        const activeResponses = this.responses
            .filter(response => response.event)
            .filter(response =>
                this.events.some(
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
        if (this.events.length !== this.recentEventsLazy.length) {
            const startSlice = this.recentEventsLazy.length
            const endSlice = this.recentEventsLazy.length + this.lazyCount - 1
            const slice = this.events.slice(startSlice, endSlice)

            this.recentEventsLazy.push(...slice)

            return true;
        } else {
            return false;
        }
    }

    showEvents = () => {
        this.store.dispatch(new ShowEvents({ events: this.events }))
    }

    getCardClass(event: Event, first: boolean) {
        return `${event.eventClusterId} ${first ? 'first' : ''}`
    }

    scrollToEvent(eventClusterId: string) {
        const eventInArr = this.recentEventsLazy.some(event => event.eventClusterId === eventClusterId);

        while (!eventInArr && this.onScroll()) { }

        setTimeout(() => {
            this.perfectScrollbar.directiveRef.scrollToElement(`.${eventClusterId}`, -5, 2000);
        })
    }
}
