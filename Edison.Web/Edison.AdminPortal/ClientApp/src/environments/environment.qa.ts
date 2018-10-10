export const environment = {
  production: true,
  mockData: false,
  authorize: true,
  mapDefaults: {
    zoom: 14,
  },
  b2c: {
    clientId: 'da85cf53-128a-47e4-a974-b4bcf0d8fb1c',
    authUrl:
      'https://login.microsoftonline.com/tfp/edisondevb2c.onmicrosoft.com/',
    policy: 'B2C_1_Edision_SignInAndSignUp',
  },
  apiUrl: 'https://edisonapi.eastus.cloudapp.azure.com/api/',
  signalRUrl: 'https://edisonapi.eastus.cloudapp.azure.com/signalr/',
  bingMapsKey:
    'Akt7a75JIqQ-QV2ZzHVP76eivabKNvlcq_JtF8zeTePsI38tt0LdAtAFeyh1MBrz',
};
