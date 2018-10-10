import { trigger, transition, style, animate } from '@angular/animations';

export const fadeMSeconds = 500;

export const fadeInOut = trigger('fadeInOut', [
  transition(':enter', [
    style({ opacity: 0 }),
    animate(fadeMSeconds, style({ opacity: 1 })),
  ]),
  transition(':leave', [style({ opacity: 1 }), animate(fadeMSeconds, style({ opacity: 0 }))]),
]);
