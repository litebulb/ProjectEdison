import { Tooltip } from './tooltip-builder';
import UUIDv1 from 'uuid/v1';

export interface IImageIcon {
    icon?: string;
    imageColor?: string;
    backgroundColor?: string;
    tooltip?: string;
    tooltipStyle?: string;
}

export const ImageIcon = (options: IImageIcon): HTMLElement => {
    return new ImageIconBuilder(options).getHtmlElement();
};

class ImageIconBuilder {
    private _iconTemplate: HTMLElement;
    private _imageBasePath: string = 'assets/icons/'
    private _iconStyle: string;
    private _imgStyle: string;
    private _domParser = new DOMParser();

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

            if (this.options.backgroundColor) {
                style += `background-color: ${this.options.backgroundColor};`;
            }
        }

        this._iconStyle = style;
        this._imgStyle = `width: 70%;`

    }

    private _getPinElement(): HTMLElement | ChildNode {
        if (this.options.icon) {
            const pinIcon = this.options.icon === 'vip' ? 'vip_white' : this.options.icon;
            const pinImage = document.createElement('IMG');

            pinImage.setAttribute('style', this._imgStyle);
            pinImage.setAttribute('src', `${this._imageBasePath}${pinIcon}.svg`);

            return pinImage;
        } else {
            return this._getSvgElement(this.options.imageColor || '#0081F9');
        }
    }

    private _buildTargetIcon() {
        const container = document.createElement('DIV');

        if (this.options.tooltip) {
            container.append(this._buildTooltipTemplate());
        }

        const imgElement = this._getPinElement();

        container.append(imgElement);
        container.setAttribute('style', this._iconStyle);

        this._iconTemplate = container;
    }

    private _getSvgElement(fillColor: string) {
        const svgCircleId = UUIDv1(); // a
        const svgPathId = UUIDv1(); // c
        const maskId = UUIDv1(); // b
        const mask2Id = UUIDv1(); // d

        // tslint:disable:max-line-length
        const svgContent = `
        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="60" height="60" viewBox="0 0 60 60">
            <defs>
                <circle id="${svgCircleId}" cx="29.887" cy="29.913" r="29.811"/>
                <path id="${svgPathId}" d="M24 48C10.745 48 0 37.255 0 24S10.745 0 24 0s24 10.745 24 24-10.745 24-24 24zm0-1.882c12.215 0 22.118-9.903 22.118-22.118 0-12.215-9.903-22.118-22.118-22.118C11.785 1.882 1.882 11.785 1.882 24c0 12.215 9.903 22.118 22.118 22.118zm0-5.862c-8.978 0-16.256-7.278-16.256-16.256 0-8.978 7.278-16.256 16.256-16.256 8.978 0 16.256 7.278 16.256 16.256 0 8.978-7.278 16.256-16.256 16.256zm0-1.275c8.274 0 14.98-6.707 14.98-14.981 0-8.274-6.706-14.98-14.98-14.98-8.274 0-14.98 6.706-14.98 14.98 0 8.274 6.706 14.98 14.98 14.98zM24 30a6 6 0 1 1 0-12 6 6 0 0 1 0 12z"/>
            </defs>
            <g fill="none" fill-rule="evenodd">
                <g opacity=".204" transform="translate(-.006)">
                    <mask id="${maskId}" fill="#fff">
                        <use xlink:href="#${svgCircleId}"/>
                    </mask>
                    <use fill="#D8D8D8" fill-rule="nonzero" xlink:href="#${svgCircleId}"/>
                    <g fill="#CBE2FF" fill-rule="nonzero" mask="url(#${maskId})">
                        <path d="M.076.102h59.859V59.96H.075z"/>
                    </g>
                </g>
                <g transform="translate(5.994 6)">
                    <mask id="${mask2Id}" fill="${fillColor}">
                        <use xlink:href="#${svgPathId}"/>
                    </mask>
                    <use fill="${fillColor}" fill-rule="nonzero" style="mix-blend-mode:lighten" xlink:href="#${svgPathId}"/>
                    <g fill="${fillColor}" fill-rule="nonzero" mask="url(#${mask2Id})">
                        <path d="M0 0h48v48H0z"/>
                    </g>
                </g>
            </g>
        </svg>
        `;
        // tslint:enable:max-line-length

        const doc = this._domParser.parseFromString(svgContent, "image/svg+xml");

        return doc.firstChild;
    }

    private _buildTooltipTemplate() {
        return Tooltip({ tooltip: this.options.tooltip, style: this.options.tooltipStyle });
    }
}
