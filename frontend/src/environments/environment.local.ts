import { globalEnvironment } from './_environment';

export const environment = {
  ...globalEnvironment(),
  production: false,
  ssl: true,
  domain: 'domain-dev.x.es',
  port: '',
  // port: '8080',

  keycloakConfig: {
    ssl: true,
    domain: 'domain-dev.x.es',
    port: '',
    // port: '8085',
    realm: '',
    clientId: '',
  },
};
