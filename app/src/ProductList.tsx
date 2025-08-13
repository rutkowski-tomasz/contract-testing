import { useQuery } from '@tanstack/react-query';
import axios from 'axios';

const fetchProducts = async () => {
  const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/products`);
  return response.data;
};

export default function ProductList() {
  const { data: products, error, isLoading, isError } = useQuery({
    queryKey: ['products'],
    queryFn: fetchProducts
  });

  if(isLoading) {
    return <div>Loading products...</div>;
  }

  if(isError) {
    return <div>Error loading products: {error.message}</div>;
  }

  return (
    <div>
      <h1>Product Catalog</h1>
      {products.length === 0 ? (
        <p>No products found.</p>
      ) : (
        <ul>
          {products.map((product: any) => (
            <li key={product.id}>
              <strong>{product.name}</strong>: {product.description} - ${product.price}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
