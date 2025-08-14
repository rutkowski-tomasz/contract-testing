import path from 'path';
import { PactV3, MatchersV3 } from '@pact-foundation/pact';
import axios from 'axios';

const { eachLike, string, integer, decimal } = MatchersV3;

const provider = new PactV3({
  consumer: 'contract-testing-app',
  provider: 'ContractTesting.Api',
  port: 1234,
  logLevel: 'info',
  dir: path.resolve(process.cwd(), 'pacts'),
});

describe('Product API Pact Tests', () => {
  // Test 1: GET /api/products - Fetch all products
  test('fetches products successfully', async () => {
    const productTemplate = {
      id: integer(1),
      name: string('Test Product'),
      description: string('Description for Test Product'),
      price: decimal(9.99),
    };

    await provider
      .given('products exist')
      .uponReceiving('a request for product list')
      .withRequest({
        method: 'GET',
        path: '/api/products',
        headers: {
          Accept: 'application/json',
        },
      })
      .willRespondWith({
        status: 200,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        },
        body: eachLike(productTemplate, 1),
      })
      .executeTest(async (mockServer) => {
        const response = await axios.get(`${mockServer.url}/api/products`, {
          headers: { Accept: 'application/json' },
        });

        expect(response.status).toBe(200);
        expect(Array.isArray(response.data)).toBe(true);
        expect(response.data.length).toBeGreaterThanOrEqual(1);
        
        const firstProduct = response.data[0];
        expect(typeof firstProduct.id).toBe('number');
        expect(Number.isInteger(firstProduct.id)).toBe(true);
        expect(typeof firstProduct.name).toBe('string');
        expect(typeof firstProduct.description).toBe('string');
        expect(typeof firstProduct.price).toBe('number');
      });
  });

  // Test 2: GET /api/products/{id} - Fetch product by ID (success case)
  test('fetches a product by ID successfully', async () => {
    const productId = 1;
    const productResponse = {
      id: integer(productId),
      name: string('Test Product'),
      description: string('Description for Test Product'),
      price: decimal(9.99),
    };

    await provider
      .given(`product with id ${productId} exists`)
      .uponReceiving('a request for a specific product by ID')
      .withRequest({
        method: 'GET',
        path: `/api/products/${productId}`,
        headers: {
          Accept: 'application/json',
        },
      })
      .willRespondWith({
        status: 200,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        },
        body: productResponse,
      })
      .executeTest(async (mockServer) => {
        const response = await axios.get(`${mockServer.url}/api/products/${productId}`, {
          headers: { Accept: 'application/json' },
        });

        expect(response.status).toBe(200);
        expect(typeof response.data.id).toBe('number');
        expect(typeof response.data.name).toBe('string');
        expect(typeof response.data.description).toBe('string');
        expect(typeof response.data.price).toBe('number');
      });
  });

  // Test 3: GET /api/products/{id} - Product not found
  test('returns 404 when product by ID does not exist', async () => {
    const productId = 999;

    await provider
      .given(`product with id ${productId} does not exist`)
      .uponReceiving('a request for a non-existent product by ID')
      .withRequest({
        method: 'GET',
        path: `/api/products/${productId}`,
        headers: {
          Accept: 'application/json',
        },
      })
      .willRespondWith({
        status: 404,
      })
      .executeTest(async (mockServer) => {
        try {
          await axios.get(`${mockServer.url}/api/products/${productId}`, {
            headers: { Accept: 'application/json' },
          });
          // If we reach here, the test should fail
          expect(true).toBe(false);
        } catch (error) {
          expect(error.response.status).toBe(404);
        }
      });
  });

  // Test 4: POST /api/products - Create a new product
  test('creates a new product successfully', async () => {
    const newProduct = {
      name: 'New Test Product',
      description: 'Description for new product',
      price: 19.99,
    };

    const createdProductResponse = {
      id: integer(1),
      name: string(newProduct.name),
      description: string(newProduct.description),
      price: decimal(newProduct.price),
    };

    await provider
      .given('no products exist with this data')
      .uponReceiving('a request to create a new product')
      .withRequest({
        method: 'POST',
        path: '/api/products',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
        },
        body: newProduct,
      })
      .willRespondWith({
        status: 201,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
          Location: string('/api/products/1'),
        },
        body: createdProductResponse,
      })
      .executeTest(async (mockServer) => {
        const response = await axios.post(`${mockServer.url}/api/products`, newProduct, {
          headers: { 
            'Content-Type': 'application/json',
            Accept: 'application/json' 
          },
        });

        expect(response.status).toBe(201);
        expect(response.headers.location).toBeTruthy();
        expect(typeof response.data.id).toBe('number');
        expect(response.data.name).toBe(newProduct.name);
        expect(response.data.description).toBe(newProduct.description);
        expect(response.data.price).toBe(newProduct.price);
      });
  });

  // Test 5: PUT /api/products/{id} - Update existing product
  test('updates an existing product successfully', async () => {
    const productId = 1;
    const updatedProduct = {
      name: 'Updated Product Name',
      description: 'Updated product description',
      price: 29.99,
    };

    const updatedProductResponse = {
      id: integer(productId),
      name: string(updatedProduct.name),
      description: string(updatedProduct.description),
      price: decimal(updatedProduct.price),
    };

    await provider
      .given(`product with id ${productId} exists`)
      .uponReceiving('a request to update an existing product')
      .withRequest({
        method: 'PUT',
        path: `/api/products/${productId}`,
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
        },
        body: updatedProduct,
      })
      .willRespondWith({
        status: 200,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        },
        body: updatedProductResponse,
      })
      .executeTest(async (mockServer) => {
        const response = await axios.put(`${mockServer.url}/api/products/${productId}`, updatedProduct, {
          headers: { 
            'Content-Type': 'application/json',
            Accept: 'application/json' 
          },
        });

        expect(response.status).toBe(200);
        expect(response.data.id).toBe(productId);
        expect(response.data.name).toBe(updatedProduct.name);
        expect(response.data.description).toBe(updatedProduct.description);
        expect(response.data.price).toBe(updatedProduct.price);
      });
  });

  // Test 6: PUT /api/products/{id} - Update non-existent product
  test('returns 404 when trying to update non-existent product', async () => {
    const productId = 999;
    const updatedProduct = {
      name: 'Updated Product Name',
      description: 'Updated product description',
      price: 29.99,
    };

    await provider
      .given(`product with id ${productId} does not exist`)
      .uponReceiving('a request to update a non-existent product')
      .withRequest({
        method: 'PUT',
        path: `/api/products/${productId}`,
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
        },
        body: updatedProduct,
      })
      .willRespondWith({
        status: 404,
      })
      .executeTest(async (mockServer) => {
        try {
          await axios.put(`${mockServer.url}/api/products/${productId}`, updatedProduct, {
            headers: { 
              'Content-Type': 'application/json',
              Accept: 'application/json' 
            },
          });
          expect(true).toBe(false);
        } catch (error) {
          expect(error.response.status).toBe(404);
        }
      });
  });

  // Test 7: DELETE /api/products/{id} - Delete existing product
  test('deletes an existing product successfully', async () => {
    const productId = 1;

    await provider
      .given(`product with id ${productId} exists`)
      .uponReceiving('a request to delete an existing product')
      .withRequest({
        method: 'DELETE',
        path: `/api/products/${productId}`,
      })
      .willRespondWith({
        status: 204,
      })
      .executeTest(async (mockServer) => {
        const response = await axios.delete(`${mockServer.url}/api/products/${productId}`);

        expect(response.status).toBe(204);
        expect(response.data).toBe('');
      });
  });

  // Test 8: DELETE /api/products/{id} - Delete non-existent product
  test('returns 404 when trying to delete non-existent product', async () => {
    const productId = 999;

    await provider
      .given(`product with id ${productId} does not exist`)
      .uponReceiving('a request to delete a non-existent product')
      .withRequest({
        method: 'DELETE',
        path: `/api/products/${productId}`,
      })
      .willRespondWith({
        status: 404,
      })
      .executeTest(async (mockServer) => {
        try {
          await axios.delete(`${mockServer.url}/api/products/${productId}`);
          expect(true).toBe(false);
        } catch (error) {
          expect(error.response.status).toBe(404);
        }
      });
  });
});