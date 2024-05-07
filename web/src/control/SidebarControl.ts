import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {PinControl} from './PinControl';
import {RenderersControl} from './RenderersControl';

export class SidebarControl {
    private readonly _livemap: LiveMap;
    private readonly _dom: HTMLElement;

    private readonly _renderersControl: RenderersControl;

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._renderersControl = new RenderersControl(this._livemap);

        this._dom = L.DomUtil.create('aside');

        if (livemap.settings.ui.sidebar.pinned != 'hide') {
            document.body.prepend(this._dom);
        }

        // set up and show/hide the pin
        const pin: PinControl = new PinControl(this._livemap, this._dom);
        this.show(pin.pinned);

        // set up the logo fancy div magic
        const holder: HTMLElement = L.DomUtil.create('div', '', this._dom);
        const logo: HTMLElement = L.DomUtil.create('div', '', holder);
        L.DomUtil.create('div', '', holder); // this one is squishy :3

        const homepage: string = this._livemap.settings.homepage;
        const link: HTMLElement = L.DomUtil.create((homepage ? 'a' : 'span'), '', logo);
        link.innerText = this._livemap.settings.lang.logo;
        link.prepend(window.createSVGIcon('logo'));
        if (homepage) {
            (link as HTMLAnchorElement).href = homepage;
        }

        // add these after the logo
        this._dom.appendChild(this.renderersControl.dom);
        this._dom.appendChild(this._livemap.playersLayer.dom);

        this._dom.onclick = (): void => {
            // todo followPlayerMarker(null)
        };
        this._dom.onmouseleave = (): void => {
            if (!pin.pinned) {
                this.show(false);
            }
        };
        this._dom.onmouseenter = (): void => {
            if (!pin.pinned) {
                this.show(true);
            }
        };
    }

    get renderersControl(): RenderersControl {
        return this._renderersControl;
    }

    public show(show: boolean): void {
        this._dom.className = show ? 'show' : '';
    }

    public tick(): void {
        //
    }
}
