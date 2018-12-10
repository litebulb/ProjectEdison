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
