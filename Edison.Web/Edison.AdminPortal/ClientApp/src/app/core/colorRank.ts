import { Event } from '../reducers/event/event.model';
import { Response, ResponseState } from '../reducers/response/response.model';

export const getRankingColor = (colors: string[]) => {
    const colorsLowered = colors.filter(c => c !== null && c !== undefined).map(c => c.toLowerCase());
    const hasRed = colorsLowered.some(c => c === 'red');
    if (hasRed) { return 'red'; }

    const hasYellow = colorsLowered.some(c => c === 'yellow');
    if (hasYellow) { return 'yellow'; }

    const hasBlue = colorsLowered.some(c => c === 'blue');
    if (hasBlue) { return 'blue'; }

    const hasGreen = colorsLowered.some(c => c === 'green');
    if (hasGreen) { return 'green'; }

    const hasGrey = colorsLowered.some(c => c === 'grey');
    if (hasGrey) { return 'grey'; }
};

export const getEventColor = (event: Event, response: Response) => {
    if (response) {
        // response is active
        if (response.responseState === ResponseState.Active) { return response.color }
        // response has been resolved
        return 'green';
    } else {
        const expired = new Date().getTime() > new Date(event.closureDate).getTime();
        if (expired) { return 'grey' } // default for a timed out response

        return 'blue' // default for an active event without a response
    }
}
