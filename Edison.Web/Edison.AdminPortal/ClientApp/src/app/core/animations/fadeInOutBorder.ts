import { trigger, transition, style, animate } from '@angular/animations';

export const fadeInOutBorder = trigger('fadeInOut', [
  transition(':enter', [
    style({ opacity: 0 }),
    animate(500, style({ opacity: .2 })),
  ]),
  transition(':leave', [animate(500, style({ opacity: 0 }))]),
]);
