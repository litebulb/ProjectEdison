import { Tooltip } from './tooltip-builder';
import { spinnerColors } from 'src/app/core/spinnerColors';

export interface SpinnerColorRgb {
    red: number;
    green: number;
    blue: number;
}

export interface ISpinnerIcon {
    tooltip?: string;
    tooltipStyle?: string;
    circleContent?: string;
    size: number;
    depth: number;
    animate?: boolean;
    spinnerColorRgb?: SpinnerColorRgb;
    spinnerStyle?: string;
    circleColor?: string;
    circleStyle?: string;
    fillColor?: string;
    fillStyle?: string;
    fontColor?: string;
    fontStyle?: string;
    color?: SpinnerColor | string;
}

export enum SpinnerColor {
    Blue = 'blue',
    Red = 'red',
    Yellow = 'yellow',
    Green = 'green',
    Grey = 'grey'
}

export const SpinnerIcon = (options: ISpinnerIcon): HTMLElement => {
    return new SpinnerIconBuilder(options).getHtmlElement();
};

class SpinnerIconBuilder {
    private _tooltipTemplate: HTMLElement;
    private _circleTemplate: HTMLElement;

    private _circleSize: string;
    private _subCircleHeight: string;
    private _subCircleWidth: string;
    private _innerCircleSize: string;
    private _innerCircleOffset: string;
    private _contentCircleSize: string;
    private _contentCircleOffset: string;
    private _subCircleGradient1: string;
    private _subCircleGradient2: string;

    private _containerStyle: string;
    private _spinnerStyle: string;
    private _outerCircleStyle: string;
    private _circleStyle: string;
    private _subCircle1Style: string;
    private _subCircle2Style: string;
    private _innerCircleStyle: string;
    private _contentCircleStyle: string;

    constructor(public options: ISpinnerIcon) {
        this._setupDefaultOptions();
        this._setup();
        this._buildStyles();
        this._buildTooltipTemplate();
        this._buildCircleTemplate();
    }

    getHtmlElement(): HTMLElement {
        return this._circleTemplate;
    }

    getTemplateString(): string {
        return this.getHtmlElement().outerHTML;
    }

    private _setupDefaultOptions() {
        const { animate, fillColor, fontColor, circleColor, spinnerColorRgb, color } = this.options;
        if (animate === undefined) { this.options.animate = true; }
        if (fillColor === undefined) { this.options.fillColor = 'white'; }
        if (fontColor === undefined) { this.options.fontColor = 'white'; }
        if (((!circleColor || !spinnerColorRgb) && !color) || !circleColor && !spinnerColorRgb && !color) {
            this.options.color = SpinnerColor.Blue; // default color to blue if no parameters are supplied
        }

        this._setupSpinnerByColor();
    }

    private _setupSpinnerByColor() {
        if (!this.options.color) { return; }

        switch (this.options.color) {
            case SpinnerColor.Red:
                this.options.circleColor = spinnerColors.redCircleColor;
                this.options.spinnerColorRgb = { red: 230, green: 44, blue: 30 };
                break;
            case SpinnerColor.Yellow:
                this.options.circleColor = spinnerColors.yellowCircleColor;
                this.options.spinnerColorRgb = { red: 255, green: 159, blue: 33 };
                break;
            case SpinnerColor.Green:
                this.options.circleColor = spinnerColors.greenCircleColor;
                this.options.spinnerColorRgb = { red: 0, green: 229, blue: 54 };
                break;
            case SpinnerColor.Grey:
                this.options.circleColor = spinnerColors.greyCircleColor;
                this.options.spinnerColorRgb = { red: 128, green: 128, blue: 128 };
                break;
            case SpinnerColor.Blue:
                this.options.circleColor = spinnerColors.blueCircleColor;
                this.options.spinnerColorRgb = { red: 0, green: 80, blue: 179 };
                break;
        }
    }

    private _buildStyles() {
        const {
            circleColor,
            circleStyle,
            fillColor,
            fillStyle,
            fontColor,
            fontStyle,
            spinnerStyle
        } = this.options;

        this._containerStyle = `
            width: ${this._circleSize};
            height: ${this._circleSize};
            position: relative;
        `;

        this._spinnerStyle = `
            position: relative;
            ${spinnerStyle}
        `;

        this._outerCircleStyle = `
            width: ${this._circleSize};
            height: ${this._circleSize};
            border-radius: 50%;
            position: absolute;
            overflow: hidden;
            animation-play-state: running;
        `;

        this._circleStyle = `
            width: ${this._circleSize};
            height: ${this._circleSize};
            border-radius: 50%;
            position: absolute;
            overflow: hidden;
            animation-name: spin;
            animation-duration: 1.16s;
            animation-iteration-count: infinite;
            animation-timing-function: cubic-bezier(0.27, 0.47, 0.67, 0.87);
            animation-play-state: running;
        `;

        this._innerCircleStyle = `
            background-color: ${fillColor};
            height: ${this._innerCircleSize};
            width: ${this._innerCircleSize};
            top: ${this._innerCircleOffset};
            left: ${this._innerCircleOffset};
            border-radius: 50%;
            position: absolute;
            ${fillStyle}
        `;

        this._subCircle1Style = `
            height: ${this._subCircleHeight};
            width: ${this._subCircleWidth};
            background: ${this._subCircleGradient1};
            transition: background-color 2s ease;
        `;

        this._subCircle2Style = `
            height: ${this._subCircleHeight};
            width: ${this._subCircleWidth};
            background: ${this._subCircleGradient2};
            transition: background-color 2s ease;
        `;

        this._contentCircleStyle = `
            top: ${this._contentCircleOffset};
            left: ${this._contentCircleOffset};
            height: ${this._contentCircleSize};
            width: ${this._contentCircleSize};
            background-color: ${circleColor};
            color: ${fontColor};
            border-radius: 50%;
            overflow: hidden;
            position: absolute;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;
            z-index: 2;
            transition: background-color 2s ease;
            ${circleStyle}
            ${fontStyle}
        `;
    }

    private _buildTooltipTemplate() {
        this._tooltipTemplate = Tooltip({ tooltip: this.options.tooltip, style: this.options.tooltipStyle });
    }

    private _buildCircleTemplate() {
        const spinner = document.createElement('DIV');
        const container = document.createElement('DIV');
        const outerCircle = document.createElement('DIV');
        const circle = document.createElement('DIV');
        const subCircle1 = document.createElement('DIV');
        const subCircle2 = document.createElement('DIV');
        const innerCircle = document.createElement('DIV');
        const contentCircle = document.createElement('DIV');

        spinner.setAttribute('style', this._spinnerStyle);
        container.setAttribute('style', this._containerStyle);
        outerCircle.setAttribute('style', this._outerCircleStyle);
        circle.setAttribute('style', this._circleStyle);
        subCircle1.setAttribute('style', this._subCircle1Style);
        subCircle2.setAttribute('style', this._subCircle2Style);
        innerCircle.setAttribute('style', this._innerCircleStyle);
        contentCircle.setAttribute('style', this._contentCircleStyle);

        if (this.options.tooltip) {
            spinner.append(this._tooltipTemplate);
        }

        spinner.append(container);
        container.append(outerCircle);
        container.append(innerCircle);
        container.append(contentCircle);
        outerCircle.append(circle);
        circle.append(subCircle1);
        circle.append(subCircle2);
        contentCircle.append(this.options.circleContent || '');

        this._circleTemplate = spinner;
    }

    private _getSpinnerRgbColorString() {
        const { spinnerColorRgb: { red, green, blue} } = this.options;
        return `${red}, ${green}, ${blue}`;
    }

    private _setup() {
        const { size, depth, animate } = this.options;

        this._circleSize = `${size}px`;
        this._subCircleHeight = `${size / 2}px`;
        this._subCircleWidth = `${size}px`;
        this._innerCircleSize = `${size - depth}px`;
        this._innerCircleOffset = `${depth / 2}px`;
        this._contentCircleSize = `${size - (depth * 2)}px`;
        this._contentCircleOffset = `${(depth * 2) / 2}px`;

        const spinnerRgb = this._getSpinnerRgbColorString();
        if (animate) {
            this._subCircleGradient1 = `linear-gradient(to right, rgba(${spinnerRgb}, .4) 0%, rgba(${spinnerRgb}, .7) 100%)`;
            this._subCircleGradient2 = `linear-gradient(to right, rgba(${spinnerRgb}, 1) 0%, rgba(${spinnerRgb}, .7) 100%)`;
        } else {
            this._subCircleGradient1 = `rgba(${spinnerRgb}, 1)`;
            this._subCircleGradient2 = `rgba(${spinnerRgb}, 1)`;
        }
    }

}
