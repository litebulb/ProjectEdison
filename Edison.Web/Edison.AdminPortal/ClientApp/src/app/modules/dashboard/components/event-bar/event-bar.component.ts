import { PerfectScrollbarComponent, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { Subscription } from 'rxjs';
import { eventsSelector } from 'src/app/reducers/event/event.selectors';

import {
    Component, ElementRef, OnDestroy, OnInit, QueryList, Renderer2, ViewChild, ViewChildren
} from '@angular/core';
import { Actions, ofType } from '@ngrx/effects';
import { select, Store } from '@ngrx/store';

import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { listFadeInOut } from '../../../../core/animations/listFadeInOut';
import { getRankingColor } from '../../../../core/colorRank';
import { spinnerColors } from '../../../../core/spinnerColors';
import { AppState } from '../../../../reducers';
import { AppPage, SetPageData } from '../../../../reducers/app/app.actions';
import {
    EventActionTypes, SelectActiveEvent, ShowEventInEventBar, ShowEvents
} from '../../../../reducers/event/event.actions';
import { Event } from '../../../../reducers/event/event.model';
import { Response, ResponseState } from '../../../../reducers/response/response.model';
import { responsesSelector } from '../../../../reducers/response/response.selectors';
import { EventCardComponent } from '../event-card/event-card.component';

@Component({
    selector: 'app-event-bar',
    templateUrl: './event-bar.component.html',
    styleUrls: [ './event-bar.component.scss' ],
    animations: [ fadeInOut, listFadeInOut ],
})
export class EventBarComponent implements OnInit, OnDestroy {
    @ViewChild(PerfectScrollbarComponent) perfectScrollbar: PerfectScrollbarComponent;
    @ViewChildren(EventCardComponent) eventCards !: QueryList<EventCardComponent>;

    activeCard: EventCardComponent;
    events: Event[]
    recentEventsLazy: Event[]
    responses: Response[] = []
    eventCount: number
    lazyCount = 3
    circleColor: string
    spinnerColor: string
    animate = true
    tabindex = 0;
    scrollConfig: PerfectScrollbarConfigInterface = {}

    private eventsSub$: Subscription
    private responsesSub$: Subscription
    private showEventInEventBarSub$: Subscription;
    private keydownListener: Function;

    constructor (private store: Store<AppState>, private actions$: Actions, private _element: ElementRef, private renderer: Renderer2) { }

    ngOnInit() {
        this.store.dispatch(new SetPageData({ title: AppPage.RightNow, showDownArrow: true, showReloadButton: true }));

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

        this.keydownListener = this.renderer.listen(this._element.nativeElement, 'keydown', (event) => {
            if (this.activeCard) {
                if (event.keyCode === 9) {
                    if (this.events.length > 1) {
                        event.preventDefault();
                        event.shiftKey ? this.tabPrevious() : this.tabNext();
                    } else {
                        this.store.dispatch(new SelectActiveEvent({ event: null }));
                    }
                }
            }
        });
    }

    ngOnDestroy() {
        this.eventsSub$.unsubscribe()
        this.responsesSub$.unsubscribe()
        this.showEventInEventBarSub$.unsubscribe();
        this.keydownListener();
    }

    onEventsUpdate = (events: Event[]) => {
        if (events) {
            this.events = events
            this.recentEventsLazy = events.filter((v, i) => i < this.lazyCount)

            this.eventCount = this.getEventCount()
            this.animate = this.eventCount > 0
        }
        this.updateSpinner()
    }

    getEventCount() {
        return this.events ? this.events
            .filter(e => e.closureDate === null)
            .reduce((a, v) => (a += v.eventCount), 0) : 0;
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

    onFocus = () => {
        if (this.events.length > 0) {
            const activeEvent = this.events[ 0 ];
            this.activeCard = this.eventCards.find(eventCard => eventCard.event.eventClusterId === activeEvent.eventClusterId);
            if (this.activeCard) {
                this.store.dispatch(new ShowEventInEventBar({ event: activeEvent }));
                this.tabindex = 1;
            }
        }
    }

    tabNext = () => {
        if (this.activeCard) {
            if (!this.activeCard.canFocus() || this.activeCard.focused) {
                if (this.events[ this.tabindex ]) {
                    const activeEvent = this.events[ this.tabindex ];
                    this.activeCard.blur();
                    this.activeCard = this.eventCards.find(eventCard => eventCard.event.eventClusterId === activeEvent.eventClusterId);
                    if (this.activeCard) {
                        this.store.dispatch(new ShowEventInEventBar({ event: this.events[ this.tabindex ] }));
                        this.tabindex += 1;
                    }
                }
            } else {
                this.activeCard.focus();
            }
        }
    }

    tabPrevious = () => {
        this.tabindex += -2;
        if (this.events[ this.tabindex ]) {
            this.store.dispatch(new ShowEventInEventBar({ event: this.events[ this.tabindex ] }));
            this.tabindex += 1;
        }
    }

    showEvents = () => {
        this.store.dispatch(new ShowEvents({ events: this.events }))
    }

    getCleanSelector(id: string) {
        return `card-anchor-${id}`;
    }

    getCardClass(event: Event, first: boolean) {
        return `${this.getCleanSelector(event.eventClusterId)} ${first ? 'first' : ''}`
    }

    scrollToEvent(eventClusterId: string) {
        const eventInArr = this.recentEventsLazy.some(event => event.eventClusterId === eventClusterId);

        while (!eventInArr && this.onScroll()) { }

        setTimeout(() => {
            this.perfectScrollbar.directiveRef.scrollToElement(`.${this.getCleanSelector(eventClusterId)}`, -5, 2000);
        })
    }
}
