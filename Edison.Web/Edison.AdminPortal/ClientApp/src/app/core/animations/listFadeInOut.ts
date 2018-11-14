import {
  trigger,
  transition,
  style,
  animate,
  query,
  stagger,
} from '@angular/animations'

export const listFadeInOut = trigger('listFadeInOut', [
  transition('* => *', [
    // each time the binding value changes
    query(':leave', [stagger(250, [animate('0.5s', style({ opacity: 0 }))])], {
      optional: true,
    }),
    query(
      ':enter',
      [
        style({ opacity: 0 }),
        stagger(250, [animate('0.5s', style({ opacity: 1 }))]),
      ],
      { optional: true }
    ),
  ]),
])
