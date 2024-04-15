import * as L from "leaflet";
import {LiveMap} from "../../LiveMap";
import {Location} from "../../data/Location";
import {MenuRow} from "./MenuRow";
import '../../scss/contextmenu';

export class ContextMenu {
    private readonly _livemap: LiveMap;
    private readonly _menu: [HTMLElement, HTMLElement];
    private readonly _rows: MenuRow[];
    private readonly _locRegex: RegExp = /\[?(?<x>-?\d+),(?<y>-?\d+)]?/;
    private readonly _format: string = '[ {x}, {z} ]';

    private _loc?: Location;
    private _cur: boolean = false;

    public constructor(livemap: LiveMap) {
        this._livemap = livemap;

        // create the menu rows
        this._rows = this.createRows();

        // create dual menus to swap between
        this._menu = [
            this.createMenu(),
            this.createMenu()
        ];

        // setup browser's listeners
        window.onblur = document.onblur = (): void => this.close();
        window.oncontextmenu = (e: MouseEvent) => this.open(e);
        window.onkeydown = (e: KeyboardEvent) => this.keydown(e);

        // setup map's listeners
        this._livemap.on('load unload resize viewreset move movestart moveend zoom zoomstart zoomend' +
            ' zoomlevelschange click dblclick mousedown mouseup preclick', (): void => this.close());
    }

    private createRows(): MenuRow[] {
        const rows: MenuRow[] = [];
        rows.push(new MenuRow('[ 0,0 ]'));
        rows.push(new MenuRow());
        if (navigator.clipboard) {
            // some browsers don't support clipboard api :(
            rows.push(new MenuRow('Copy', 'copy', 'Ctrl+C', () => this.copy()));
            rows.push(new MenuRow('Paste', 'paste', 'Ctrl+V', () => this.paste()));
            rows.push(new MenuRow());
            rows.push(new MenuRow('Share', 'link', 'Ctrl+S', () => this.share()));
            rows.push(new MenuRow());
        }
        rows.push(new MenuRow('Center', 'center', 'F10', () => this.center()));
        return rows;
    }

    private createMenu(): HTMLElement {
        // create the menu
        const wrapper: HTMLElement = L.DomUtil.create('div', 'wrapper');
        const menu: HTMLElement = document.body.appendChild(wrapper)
            .appendChild(L.DomUtil.create('div'))
            .appendChild(L.DomUtil.create('div', 'contextmenu'));

        // add the rows to the menu
        this._rows.forEach((row: MenuRow): void => menu.append(row.create()));

        // prevent menu from opening real context menu
        menu.addEventListener('contextmenu', (e: Event): void => this.stopPropagation(e));

        return wrapper;
    }

    private close(): void {
        // hide the current menu
        this._menu[+this._cur].classList.remove('show');

        this._loc = undefined;
    }

    private open(e: MouseEvent): void {
        // close current menu (mobile needs this)
        this.close();

        // show the next menu at mouse position, while keeping it inside the viewable area
        const menu: HTMLElement = this._menu[+(this._cur = !this._cur)];
        menu.classList.add('show');
        menu.style.top = `${Math.min(window.innerHeight - menu.offsetHeight - 25, e.y)}px`;
        menu.style.left = `${Math.min(window.innerWidth - menu.offsetWidth - 25, e.x)}px`;

        // stop the event from bubbling up the stack
        this.stopPropagation(e);

        // update coordinates in first row
        this._loc = this._livemap.coordsControl.getLocation();
        menu.querySelector('div:first-child p:nth-child(2)')!.innerHTML = this._loc.toString(this._format);
    }

    private keydown(e: KeyboardEvent): void {
        // close menu when escape is pressed
        if (e.key === 'Escape') {
            this.close();
            return;
        }

        // get current key combo
        const combo: string = this.combo(e);

        // find and run any actions for this key combo
        this._rows.forEach((row: MenuRow): void => {
            if (row.key == combo && row.action) {
                row.action(e);
                this.stopPropagation(e);
            }
        });
    }

    private combo(e: KeyboardEvent): string {
        if (e.key == 'Control' || e.key == 'Shift' || e.key == 'Alt') {
            return '';
        }

        let combo: string = '';
        if (e.ctrlKey) {
            combo += `${combo ? '+' : ''}ctrl`;
        }
        if (e.shiftKey) {
            combo += `${combo ? '+' : ''}shift`;
        }
        if (e.altKey) {
            combo += `${combo ? '+' : ''}alt`;
        }
        combo += `${combo ? '+' : ''}${e.key}`;
        return combo.toLowerCase();
    }

    public copy(): void {
        navigator.clipboard.writeText((this._loc ?? this._livemap.coordsControl.getLocation()).toString(this._format))
            .then((): void => {
                this._livemap.notifications.success('Copied location to clipboard');
            })
            .catch((e): void => {
                console.error('Could not copy location\n', e);
                this._livemap.notifications.danger('Could not copy location');
            })
            .finally((): void => {
                this.close();
            });
    }

    public paste(): void {
        navigator.clipboard.readText()
            .then((text: string): void => {
                const loc: Location | undefined = this.pasteUrl(text) ?? this.pasteLocation(text);
                if (loc) {
                    this._livemap.notifications.info('Centered on location from clipboard');
                    this._livemap.centerOn(loc);
                } else {
                    this._livemap.notifications.warning('Not a valid location');
                }
            })
            .catch((e): void => {
                console.error('Could not paste location\n', e);
                this._livemap.notifications.danger('Could not paste location');
            })
            .finally((): void => {
                this.close();
            });
    }

    private pasteLocation(text: string): Location | undefined {
        const match: RegExpExecArray | null = this._locRegex.exec(text.replace(/\s+/g, ''));
        if (match) {
            return Location.of(parseInt(match[1]), parseInt(match[2]));
        }
    }

    private pasteUrl(text: string): Location | undefined {
        try {
            const params: URLSearchParams = new URLSearchParams(new URL(text).searchParams);
            return Location.of(parseInt(params.get('x') ?? ''), parseInt(params.get('z') ?? ''));
        } catch (err) {
            return;
        }
    }

    public share(loc?: Location): void {
        loc ??= this._loc ?? this._livemap.coordsControl.getLocation();
        const text: string = `${window.location.origin}${window.location.pathname}?x=${loc.x}&z=${loc.z}&zoom=${this._livemap.currentZoom()}`;
        navigator.clipboard.writeText(text)
            .then((): void => {
                this._livemap.notifications.success('Copied shareable url to clipboard');
            })
            .catch((e): void => {
                console.error('Could not copy shareable url\n', e);
                this._livemap.notifications.danger('Could not copy shareable url');
            })
            .finally((): void => {
                this.close();
            });
    }

    public center(): void {
        this._livemap.centerOn(this._livemap.coordsControl.getLocation());
        this._livemap.coordsControl.update();
        this._livemap.linkControl.update();
        this.close();
    }

    private stopPropagation(e: Event): void {
        // stop the event in the current element
        // and prevent it from bubbling up the stack
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
    }
}
