import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar';
import { CartSidebarComponent } from './components/cart-sidebar/cart-sidebar';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, CartSidebarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  readonly appStatus = environment.appStatus;
}
