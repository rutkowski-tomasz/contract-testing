import path from 'path';
import { Pact } from '@pact-foundation/pact';
import axios from 'axios';

const provider = new Pact({
  consumer: 'contract-testing-app',
  provider: 'ContractTesting.Api',
  port: 1234,
  log: path.resolve(process.cwd(), 'logs', 'pact.log'),
  dir: path.resolve(process.cwd(), 'pacts'),
  logLevel: 'info',
  spec: 2,
});

describe('Product List Pact', () => {
  beforeAll(() => provider.setup());

  afterAll(() => provider.finalize());

  afterEach(() => provider.verify());

  test('fetches products successfully', async () => {
    await provider.addInteraction({
      state: 'products exist',
      uponReceiving: 'a request for product list',
      withRequest: {
        method: 'GET',
        path: '/api/products',
        headers: {
          Accept: 'application/json',
        },
      },
      willRespondWith: {
        status: 200,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        },
        body: [
          {
            id: 1,
            name: 'Test Product',
            description: 'Description for Test Product',
            price: 9.99,
          },
        ],
      },
    });

    const response = await axios.get('http://localhost:1234/api/products', {
      headers: { Accept: 'application/json' },
    });

    expect(response.status).toBe(200);
    expect(response.data).toEqual([
      {
        id: 1,
        name: 'Test Product',
        description: 'Description for Test Product',
        price: 9.99,
      },
    ]);
  });
});
