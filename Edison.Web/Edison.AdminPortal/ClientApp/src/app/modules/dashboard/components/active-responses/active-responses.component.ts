import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';

import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { SearchListItem } from '../../../../core/models/searchListItem';
import { AddEditAction } from '../../../../reducers/action-plan/action-plan.model';
import { Response, ResponseState } from '../../../../reducers/response/response.model';

enum ActiveView {
    Default,
    Deactivate,
    Manage,
    Update,
}

@Component({
    selector: 'app-active-responses',
    templateUrl: './active-responses.component.html',
    styleUrls: [ './active-responses.component.scss' ],
    animations: [ fadeInOut ],
})
export class ActiveResponsesComponent implements OnChanges {
    @Input() activeResponse: Response;
    @Input() activeResponses: Response[] = [];
    @Input() responses: Response[] = [];

    @Output() onSelectActiveResponse = new EventEmitter<Response>();
    @Output() onGetFullResponse = new EventEmitter<string>();
    @Output() onToggleOverlay = new EventEmitter<boolean>();
    @Output() onDeactivateResponse = new EventEmitter<string>();
    @Output() onRetryResponseActions = new EventEmitter<string>();
    @Output() onResponseActionsUpdated = new EventEmitter<{ response: Response, actions: AddEditAction[], isCloseAction: boolean }>();
    
    items: SearchListItem[];
    active = false;
    activeView = ActiveView.Default;
    loadingFullResponse = false;
    scrollConfig = { suppressScrollX: true, suppressScrollY: false, useBothWheelAxes: true, scrollIndicators: true };

    ngOnChanges() {
        this._updateActiveResponses();
        this._getFullResponse();
    }

    deactivateResponse(responseId: string) { this.onDeactivateResponse.emit(responseId); }

    onActionsUpdated(payload: { actions: AddEditAction[], response: Response }, isCloseAction: boolean) {
        this.onResponseActionsUpdated.emit({
            actions: payload.actions,
            response: payload.response,
            isCloseAction,
        });
    }

    getIconStyle(index: number) {
        if (index < 5) { return null; }

        const style = {
            'margin-right': '10px',
            'margin-top': '6px',
        };

        return style;
    }

    selectActiveResponse(item: SearchListItem) {
        if (item) {
            const activeResponse = this.activeResponses.find(r => r.responseId === item.id);
            this.onSelectActiveResponse.emit(activeResponse);
            this._getFullResponse();
        } else {
            this.onSelectActiveResponse.emit(null);
        }
        this._showView(ActiveView.Default);
    }

    openResponse(response: Response) {
        this.active = true;
        this.onSelectActiveResponse.emit(response);
        this._showView(ActiveView.Default);

        this._getFullResponse()
        this.onToggleOverlay.emit(true);
    }

    showDeactivateView() { this._showView(ActiveView.Deactivate); }

    showUpdateView() { this._showView(ActiveView.Update); }

    backClicked() { this._showView(ActiveView.Default); }

    toggle(active: boolean) {
        this.active = active;
        this.onToggleOverlay.emit(this.active);

        if (active) {
            this._showView(ActiveView.Default);
            this._getFullResponse();
        }
    }

    toggleResponses() {
        this.active = !this.active

        if (!this.active) { this.onSelectActiveResponse.emit(null); }

        if (!this.active) {
            this._refreshResponses(this.activeResponses) // clear out any resolved responses on close
        }

        this.onToggleOverlay.emit(this.active);
    }

    retryResponseActions(responseId: string) { this.onRetryResponseActions.emit(responseId); }

    private _updateActiveResponses() {
        let responsesToShow = [ ...this.activeResponses ]
        if (this.activeResponse) {
            const activeResponse = this.responses.find(response => response.responseId === this.activeResponse.responseId)
            if (!activeResponse) {
                // we want to let this response still show until the active responses window closes
                const currentlyActiveResponse = {
                    ...this.activeResponse,
                    responseState: ResponseState.Inactive,
                };

                responsesToShow = [ ...this.activeResponses, currentlyActiveResponse ]
                    .filter(resp => resp.name).sort((a, b) => a.name.localeCompare(b.name))
            } else {
                this._getFullResponse();
            }
        }

        this._refreshResponses(responsesToShow)
    }

    private _refreshResponses(responses: Response[]) {
        this.items = responses.map(r => ({
            id: r.responseId,
            name: r.name,
            icon: r.icon,
            color: r.color,
        }));
    }

    private _showView(activeView: ActiveView) { this.activeView = activeView; }

    private _getFullResponse() {
        if (this.activeResponse && !this.loadingFullResponse && !this.activeResponse.actionPlan) {
            this.loadingFullResponse = true;
            this.onGetFullResponse.emit(this.activeResponse.responseId);
        } else if (this.activeResponse && this.activeResponse.actionPlan) {
            this.loadingFullResponse = false;
        }
    }
}
