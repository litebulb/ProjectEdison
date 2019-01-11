import {
    Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild
} from '@angular/core';

import { ActiveUser } from '../../../../core/models/activeUser';
import { MessageModel } from '../../../../core/services/directline/models/activity-model';
import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';
import { Event } from '../../../../reducers/event/event.model';

@Component({
    selector: 'app-recently-active',
    templateUrl: './recently-active.component.html',
    styleUrls: [ './recently-active.component.scss' ],
})
export class RecentlyActiveComponent implements OnInit {
    @ViewChild('header') header: ElementRef;

    @Input() activeUsers: ActiveUser[];
    @Input() actionPlans: ActionPlan[];
    @Input() events: Event[];
    @Input() messages: MessageModel[];
    @Input() activeUser: { name: string, userId: string };

    @Output() activeUserSelected = new EventEmitter();
    @Output() openAllChatClick = new EventEmitter();

    ngOnInit() {
        this.header.nativeElement.focus();
    }

    getUnreadMessageCount(userId: string) {
        return this.messages.filter(m => m.channelData.data.userId === userId && !m.read).length;
    }

    getActionPlan(userId: string) {
        const actionPlanId = this._getLatestActionPlanId(userId);
        if (actionPlanId) {
            return this.actionPlans.find(ap => ap.actionPlanId === actionPlanId);
        }

        return null;
    }

    selectActiveUser(user: ActiveUser) {
        this.activeUserSelected.emit(user);
    }

    selectAllChat() {
        this.openAllChatClick.emit();
    }

    private _getLatestActionPlanId(userId: string) {
        const event = this.events.find(event => event.events.some(ee => ee.metadata.userId === userId));
        if (event) {
            const latestEventInstance = event.events
                .sort((a, b) => (new Date(b.date).getTime() - new Date(a.date).getTime()))[ 0 ];
            if (latestEventInstance) {
                return latestEventInstance.metadata.reportType;
            }
        }

        return null;
    }
}
