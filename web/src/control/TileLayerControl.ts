import {LiveMap} from '../LiveMap';
import {LiveTileLayer} from '../layer/LiveTileLayer';

export class TileLayerControl {
    private readonly _livemap: LiveMap;

    private readonly _layers: LiveTileLayer[] = [];

    private _cur: boolean = false;

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        // hardcode for now - implementation subject to change
        const renderer: string = "basic";

        // we need 2 tile layers to swap between for seamless refreshing
        livemap.addLayer(this._layers[0] = this.createTileLayer(livemap, renderer));
        livemap.addLayer(this._layers[1] = this.createTileLayer(livemap, renderer));
    }

    public tick(count: number): void {
        if (count % this._livemap.settings.interval == 0) {
            this.updateTileLayer();
        }
    }

    private createTileLayer(livemap: LiveMap, renderer: string): LiveTileLayer {
        return new LiveTileLayer(livemap, renderer)
            .addEventListener('load', (): void => {
                // switch layers when all tiles are loaded
                this.switchTileLayer();
            });
    }

    private switchTileLayer(): void {
        // swap tile layers
        this._layers[+this._cur].setZIndex(0);
        this._layers[+!this._cur].setZIndex(1);
        this._cur = !this._cur;
    }

    public updateTileLayer(): void {
        // redraw (reload images) current tile layer
        this._layers[+this._cur].redraw();
    }
}
