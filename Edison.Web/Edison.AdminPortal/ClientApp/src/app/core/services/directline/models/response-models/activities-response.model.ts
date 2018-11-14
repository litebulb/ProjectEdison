import { ActivityModel } from "../activity-model";

export interface ActivitiesResponseModel {
    activities: ActivityModel[];
    watermark: string;
}
