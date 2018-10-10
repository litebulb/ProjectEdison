// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  mockData: false,
  authorize: true,
  mapDefaults: {
    zoom: 14,
  },
  b2c: {
    clientId: '1133966b-9c18-4edb-9efc-0d0c01494e6b',
    authUrl:
      'https://login.microsoftonline.com/tfp/edisondevb2c.onmicrosoft.com/',
    policy: 'B2C_1_Edision_SignInAndSignUp',
  },
  apiUrl: 'https://edisonapidev.eastus.cloudapp.azure.com/api/',
  signalRUrl: 'https://edisonapidev.eastus.cloudapp.azure.com/signalr/',
  bingMapsKey:
    'Akt7a75JIqQ-QV2ZzHVP76eivabKNvlcq_JtF8zeTePsI38tt0LdAtAFeyh1MBrz',
};

/*
 * In development mode, for easier debugging, you can ignore zone related error
 * stack frames such as `zone.run`/`zoneDelegate.invokeTask` by importing the
 * below file. Don't forget to comment it out in production mode
 * because it will have a performance impact when errors are thrown
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
