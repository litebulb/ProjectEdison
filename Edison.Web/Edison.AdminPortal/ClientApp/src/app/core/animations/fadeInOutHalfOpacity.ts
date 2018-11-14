import { trigger, transition, style, animate } from '@angular/animations';

export const fadeMSeconds = 500;

export const fadeInOutHalfOpacity = trigger('fadeInOutHalfOpacity', [
  transition(':enter', [
    style({ opacity: 0 }),
    animate(fadeMSeconds, style({ opacity: .5 })),
  ]),
  transition(':leave', [style({ opacity: .5 }), animate(fadeMSeconds, style({ opacity: 0 }))]),
]);
