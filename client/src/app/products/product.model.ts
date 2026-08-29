export interface Product {
  productId: number;
  productName: string;
  price: number;
  unitsInStock: number;
  categoryId: number;
  categoryName: string;
}

export interface ProductRequest {
  productName: string;
  price: number;
  unitsInStock: number;
  categoryId: number;
}
