import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';

// TODO: Update to use observable instead of setInterval

@Component({
  selector: 'app-delay-button',
  templateUrl: './delay-button.component.html',
  styleUrls: ['./delay-button.component.scss'],
})
export class DelayButtonComponent {
  @Input() buttonText: string;
  @Input() subheaderText: string;
  @Input() disabled = false;
  @Output() clickCompleted = new EventEmitter();

  currentProgress = 0;
  maxProgress = 1000;
  progressMaxed = false;
  progressInterval: any;

  onMouseDown = () => {
    if (this.disabled) { return; }

    clearInterval(this.progressInterval);
    this.progressInterval = setInterval(() => { this.incrementProgress(); }, 1);
  }

  onMouseUp = () => {
    if (this.disabled) { return; }

    clearInterval(this.progressInterval);
    if (!this.progressMaxed) {
      this.progressInterval = setInterval(() => { this.decrementProgress(); }, 1);
    }
  }

  incrementProgress() {
    if (this.currentProgress < this.maxProgress) {
      this.currentProgress += 1;
    } else {
      clearInterval(this.progressInterval);
      this.progressMaxed = true;
      this.clickCompleted.emit();
    }
  }

  decrementProgress() {
    if (this.currentProgress > 0) {
      this.currentProgress -= 1;
    } else {
      clearInterval(this.progressInterval);
    }
  }

}
