import { Event } from '../reducers/event/event.model';
import { Response, ResponseState } from '../reducers/response/response.model';
import { SpinnerColor } from '../modules/map/helpers/spinner-icon-builder';

export const getRankingColor = (colors: string[]) => {
  const colorsLowered = colors
    .filter(c => c !== null && c !== undefined)
    .map(c => c.toLowerCase());
  const hasRed = colorsLowered.some(c => c === 'red');
  if (hasRed) {
    return SpinnerColor.Red;
  }

  const hasYellow = colorsLowered.some(c => c === 'yellow');
  if (hasYellow) {
    return SpinnerColor.Yellow;
  }

  const hasBlue = colorsLowered.some(c => c === 'blue');
  if (hasBlue) {
    return SpinnerColor.Blue;
  }

  const hasGreen = colorsLowered.some(c => c === 'green');
  if (hasGreen) {
    return SpinnerColor.Green;
  }

  const hasGrey = colorsLowered.some(c => c === 'grey');
  if (hasGrey) {
    return SpinnerColor.Grey;
  }
};

export const getEventColor = (
  event: Event,
  response: Response
): SpinnerColor | string => {
  if (response) {
    // response is active
    if (response.responseState === ResponseState.Active) {
      return response.color;
    }
    // response has been resolved
    return SpinnerColor.Green;
  } else if (event.closureDate) {
    const expired =
      new Date().getTime() > new Date(event.closureDate).getTime();
    if (expired) {
      return SpinnerColor.Grey;
    } // default for a timed out response
  }

  return SpinnerColor.Blue; // default for an active event without a response
};
