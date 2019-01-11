export interface ITooltip {
    tooltip?: string;
    style?: string;
}

export const Tooltip = (options: ITooltip): HTMLElement => {
    return new TooltipBuilder(options).getHtmlElement();
}

class TooltipBuilder {
    private _tooltipTemplate: HTMLElement;
    private _tooltipStyle: string;
    private _tooltipContainerStyle: string;
    private _downArrowStyle: string;

    constructor(public options: ITooltip) {
        this._buildStyles();
        this._buildTooltipTemplate();
    }

    public getHtmlElement(): HTMLElement {
        return this._tooltipTemplate;
    }

    public getStringTemplate(): string {
        return this._tooltipTemplate.outerHTML;
    }

    private _buildStyles() {
        const { style } = this.options;


        this._tooltipContainerStyle = `
            height: 30px;
            display: flex;
            justify-content: center;
            align-items: center;
            position: absolute;
            top: -40px;
            width: 100%;
            opacity: 0.6;
            z-index: 3;
        `

        this._tooltipStyle = `
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: black;
            border-radius: 4px;
            font-size: 14px;
            color: white;
            padding: 5px 20px;
            white-space: nowrap;
            ${style}
        `

        this._downArrowStyle = `
            width: 0;
            height: 0;
            border-left: 10px solid transparent;
            border-right: 10px solid transparent;
            border-top: 10px solid black;
            position: absolute;
            bottom: -6px;
        `
    }

    private _buildTooltipTemplate() {
        const container = document.createElement("DIV");
        const tooltip = document.createElement("DIV");
        const downArrow = document.createElement("DIV");

        container.setAttribute('style', this._tooltipContainerStyle);
        tooltip.setAttribute('style', this._tooltipStyle);
        downArrow.setAttribute('style', this._downArrowStyle);

        container.append(tooltip);
        container.append(downArrow);

        tooltip.append(this.options.tooltip);

        this._tooltipTemplate = container;
    }
}
