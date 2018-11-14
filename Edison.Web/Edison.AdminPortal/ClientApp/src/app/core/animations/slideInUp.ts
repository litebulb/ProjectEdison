import {
    trigger,
    transition,
    style,
    animate,
} from '@angular/animations'

export const slideInUp = trigger('slideInUp', [
    transition(':enter', [
        style({ transform: 'translateY(100%)' }),
        animate('500ms ease-in', style({ transform: 'translateY(0%)' })),
    ])
    // transition(':leave', [ style({ opacity: 1 }), animate(500, style({ opacity: 0 })) ]),
])
