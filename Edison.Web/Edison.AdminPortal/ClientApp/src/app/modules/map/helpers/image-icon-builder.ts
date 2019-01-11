import { Tooltip } from './tooltip-builder';

export interface IImageIcon {
    icon?: string;
    color?: string;
    tooltip?: string;
    tooltipStyle?: string;
}

export const ImageIcon = (options: IImageIcon): HTMLElement => {
    return new ImageIconBuilder(options).getHtmlElement();
}

class ImageIconBuilder {
    private _iconTemplate: HTMLElement;
    private _imageBasePath: string = 'assets/icons/'
    private _iconStyle: string;

    constructor(public options: IImageIcon) {
        this._setupStyles();
        this._buildTooltipTemplate();
        this._buildTargetIcon();
    }

    getHtmlElement(): HTMLElement {
        return this._iconTemplate;
    }

    getTemplateString(): string {
        return this.getHtmlElement().outerHTML;
    }

    private _setupStyles() {
        let style = 'cursor: pointer;';

        if (this.options.icon) {
            style += `
                cursor: pointer;
                border-radius: 50%;
                width: 40px;
                height: 40px;
                display: flex;
                align-items: center;
                justify-content: center;
            `;

            let bgColor = '';

            if (this.options.color) {
                switch (this.options.color.toLowerCase()) {
                    case 'blue':
                        bgColor = '#3A82FE'
                        break;
                    case 'green':
                        bgColor = '#00E536'
                        break;
                    case 'red':
                        bgColor = '#FA4035'
                        break;
                    case 'yellow':
                        bgColor = '#FABF0D'
                        break;
                }
                style += `background-color: ${bgColor};`;
            }
        }

        this._iconStyle = style;
    }

    private _getImgSrc() {
        let image = 'pin';
        if (this.options.icon) {
            image = this.options.icon === 'vip' ? 'vip_white' : this.options.icon;
        }
        return `${this._imageBasePath}${image}.svg`;
    }

    private _buildTargetIcon() {
        const container = document.createElement("DIV");
        const img = document.createElement("IMG");

        if (this.options.tooltip) {
            container.append(this._buildTooltipTemplate())
        }

        container.append(img);

        img.setAttribute('style', this._iconStyle);
        img.setAttribute('src', this._getImgSrc());

        this._iconTemplate = container;
    }

    private _buildTooltipTemplate() {
        return Tooltip({ tooltip: this.options.tooltip, style: this.options.tooltipStyle });
    }
}
