export const environment = {
  production: true,
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
