import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../environments/environment';
import { ProductList } from '../models/product.model';
import { ProductService } from './product.service';

describe('ProductService', () => {
  let service: ProductService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(ProductService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('encodes the search query and sends it to the products search endpoint', () => {
    const response: ProductList = {
      products: [],
      totalCount: 0,
    };

    let emittedResponse: ProductList | undefined;
    service.search('usb c hub').subscribe((value) => {
      emittedResponse = value;
    });

    const request = httpTesting.expectOne(`${environment.apiUrl}/products/search?q=usb%20c%20hub`);
    expect(request.request.method).toBe('GET');

    request.flush(response);

    expect(emittedResponse).toEqual(response);
  });
});
