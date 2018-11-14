import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { MapComponent } from './components/map/map.component'
import { ZoomComponent } from './components/zoom/zoom.component'
import { MaterialModule } from '../material/material.module'
import { SharedModule } from '../shared/shared.module'
import { SelectLocationComponent } from './components/select-location/select-location.component';

@NgModule({
    imports: [ CommonModule, MaterialModule, SharedModule ],
    declarations: [ MapComponent, ZoomComponent, SelectLocationComponent ],
    exports: [ MapComponent ],
})
export class MapModule { }
