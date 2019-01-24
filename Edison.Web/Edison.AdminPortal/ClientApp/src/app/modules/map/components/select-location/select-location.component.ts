import {
  Component,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  Input,
  OnChanges,
} from '@angular/core';

enum SelectionState {
  Default = 'The response you activated does not have a location on the map, would you like to set one?',
  Selecting = 'Select a spot on map where you want to set the response location.',
  Confirm = 'You set a response location. Good to go?',
}

@Component({
  selector: 'app-select-location',
  templateUrl: './select-location.component.html',
  styleUrls: ['./select-location.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SelectLocationComponent implements OnChanges {
  selectionStates = SelectionState;
  selectionState: SelectionState = SelectionState.Default;

  @Output() onCancel = new EventEmitter();
  @Output() onSetLocation = new EventEmitter();
  @Output() onConfirmLocation = new EventEmitter();
  @Output() onRestart = new EventEmitter();

  @Input() confirmSelection: boolean;

  ngOnChanges() {
    if (this.confirmSelection) {
      this.selectionState = SelectionState.Confirm;
    }
  }

  setLocation() {
    this.selectionState = SelectionState.Selecting;
    this.onSetLocation.emit();
  }

  cancel() {
    this.selectionState = SelectionState.Default;
    this.onCancel.emit();
  }

  restart() {
    this.selectionState = SelectionState.Default;
    this.onRestart.emit();
  }

  confirmLocation() {
    this.selectionState = SelectionState.Default;
    this.onConfirmLocation.emit(false);
  }
}
