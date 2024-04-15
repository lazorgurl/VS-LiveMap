import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {Marker, MarkerJson, PopupOptions} from "./marker/Marker";
import {Circle} from "./marker/Circle";
import {Ellipse} from "./marker/Ellipse";
import {Icon} from "./marker/Icon";
import {Polygon} from "./marker/Polygon";
import {Polyline} from "./marker/Polyline";
import {Rectangle} from "./marker/Rectangle";

interface Defaults {
    options: L.LayerOptions,
    popup: PopupOptions,
    tooltip: L.TooltipOptions
}

interface LayerJson {
    label: string,
    interval: number,
    hidden: boolean,
    options: L.LayerOptions,
    defaults: Defaults,
    markers: MarkerJson[],
    css: string
}

export class MarkersLayer extends L.LayerGroup {
    private readonly _livemap: LiveMap;
    private readonly _url: string;

    private readonly _markers: Map<string, Marker> = new Map<string, Marker>();

    private _id?: string;
    private _label?: string;
    private _interval?: number;
    private _defaults?: Defaults;
    private _json?: LayerJson;

    constructor(livemap: LiveMap, url: string) {
        super([]);
        this._livemap = livemap;
        this._url = url;

        // initial update to get interval and label
        this.updateLayer();
    }

    get id(): string {
        return this._id!;
    }

    get label(): string {
        return this._label!;
    }

    get interval(): number {
        return this._interval!;
    }

    get json(): LayerJson {
        return this._json!;
    }

    public tick(count: number): void {
        if (count % (this._interval ?? 0) == 0) {
            this.updateLayer();
        }
    }

    private initial(json: LayerJson): void {
        this._label = json.label ?? ''; // set _something_ so we don't keep reloading json every tick
        this._interval = json.interval ?? 300;
        this._defaults = json.defaults;
        this._json = json;

        // merge in the custom layer options
        if (json.options) {
            this.options = {
                ...this.options,
                ...json.options
            };

            // create any panes needed for this marker
            this._livemap.createPaneIfNotExist(json.options?.pane);
        }

        // insert any custom css
        if (json.css) {
            document.head.insertAdjacentHTML('beforeend', `<style id="${this.id}">${json.css}</style>`);
        }

        // only add to the map if we are not hiding it by default
        if (!json.hidden) {
            // adding to the map makes it visible
            this.addTo(this._livemap);
        }

        // add this layer to the layers control
        this._livemap.layersControl.addOverlay(this, this._label);
    }

    private updateLayer(): void {
        window.fetchJson<LayerJson>(this._url)
            .then((json: LayerJson): void => {
                if (!this._label) {
                    // this is the first tick
                    this.initial(json);
                }

                // refresh markers
                this.updateMarkers(json);
            })
            .catch((err: unknown): void => {
                console.error(`Error updating markers layer (${this._label})\n`, this, err);
            });
    }

    private updateMarkers(layerJson: LayerJson): void {
        const toRemove: string[] = [...this._markers.keys()];

        // get all markers from json
        layerJson.markers.forEach((markerJson: MarkerJson): void => {
            try {
                const marker: Marker | undefined = this._markers.get(markerJson.id);
                if (marker) {
                    // update existing marker
                    marker.update(markerJson);
                    toRemove.remove(markerJson.id);
                } else {
                    // create new marker
                    this.createMarker(markerJson);
                }
            } catch (e) {
                console.error(`Error refreshing markers in layer (${this._label})\n`, this, markerJson, e);
            }
        });

        // any markers left over are no longer in the json file and must be removed
        toRemove.forEach((key: string): void => {
            this._markers.get(key)?.remove();
            this._markers.delete(key);
        });
    }

    private createMarker(json: MarkerJson): void {
        // merge in default options, if any
        if (this._defaults) {
            if (this._defaults.options) {
                json.options = {...this._defaults.options, ...json.options};
            }
            if (this._defaults.popup) {
                json.popup = {...this._defaults.popup, ...json.popup};
            }
            if (this._defaults.tooltip) {
                json.tooltip = {...this._defaults.tooltip, ...json.tooltip};
            }
        }

        // set to correct pane from layer if needed
        const layerPane: string | undefined = this.json.options?.pane;
        if (layerPane !== undefined) {
            // layer has custom pane set
            if (json.options?.pane === undefined) {
                // marker does not have custom pane, use layer's instead
                json.options = {
                    ...json.options,
                    pane: layerPane
                }
            }
        }

        // create new marker from json, add to layer, and store
        this._markers.set(json.id, this.createType(json).addTo(this));
    }

    private createType(json: MarkerJson): Marker {
        switch (json.type) {
            case 'circle':
                return new Circle(this, json);
            case 'ellipse':
                return new Ellipse(this, json);
            case 'icon':
                return new Icon(this, json);
            case 'polygon':
                return new Polygon(this, json);
            case 'polyline':
                return new Polyline(this, json);
            case 'rectangle':
                return new Rectangle(this, json);
        }
        throw new Error(`Invalid marker type (${json.type})`);
    }
}
