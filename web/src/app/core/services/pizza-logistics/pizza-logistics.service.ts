import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { DriverState } from '../../../shared/enums/driver-state.enum';
import { PizzaState } from '../../../shared/enums/pizza-state.enum';
import { Driver } from '../../../shared/models/driver.model';
import { PizzaOrder } from '../../../shared/models/pizza-order.model';
import { PizzaSize } from '../../../shared/models/pizza-size.model';
import { PizzaTopping } from '../../../shared/models/pizza.topping';
import { OrdersService } from '../orders/orders.service';

@Injectable({
  providedIn: 'root'
})
export class PizzaLogisticsService {
  private orders: PizzaOrder[] = [];

  private orders$: ReplaySubject<PizzaOrder[]> = new ReplaySubject<PizzaOrder[]>();

  constructor(
    private ordersService: OrdersService) {
  }

  public createNewOrder(order: PizzaOrder): void {
    order.state = PizzaState.open;
    this.ordersService.placeOrder(order).subscribe(
      data => this.updateOrders(data),
      error => console.log(error)
    );
  }

  public getOrders(): ReplaySubject<PizzaOrder[]> {
    if (this.orders.length === 0) {
      this.ordersService.getOrders().subscribe(
        data => {
          this.orders = data;
          this.orders$.next(this.orders);
        },
        error => console.log(error)
      );
    }

    return this.orders$;
  }

  public getDrivers(): ReplaySubject<Driver[]> {
    // TODO: Implement functionality to return the drivers.
  }

  public getDriverById(id: string): Driver {
    return this.drivers.find(x => x.id === id);
  }

  public getToppings(): ReplaySubject<PizzaTopping[]> {
    // TODO: Implement functionality to return the toppings.
  }

  public getToppingById(id: number): PizzaTopping {
    return this.toppings.find(x => x.id === id);
  }

  public getSizes(): ReplaySubject<PizzaSize[]> {
    // TODO: Implement functionality to return the sizes.
  }

  public placeNewOrder(pizzaOrder: PizzaOrder): void {
    this.ordersService.placeOrder(pizzaOrder).subscribe(
      data => this.updateOrders(data),
      error => console.log(error)
    );
  }

  public sendToKitchen(pizzaOrder: PizzaOrder): void {
    this.cookPizza(pizzaOrder);
  }

  public assignDriver(pizzaOrder: PizzaOrder, driverId: string): void {
    this.deliverPizza(pizzaOrder, driverId);
  }

  private updateOrders(pizzaOrder: PizzaOrder): void {
    this.orders.push(pizzaOrder);
    this.orders$.next(this.orders);
  }

  private setDriverState(driverId: string, state: DriverState): void {
    // TODO: Implement functionality to set the driver state.
  }

  private setOrderState(pizzaOrder: PizzaOrder, state: PizzaState): void {
    const pizzaOrderInArray = this.orders.find(x => x.id === pizzaOrder.id);
    pizzaOrderInArray.state = state;

    this.ordersService.updateOrder(pizzaOrderInArray);
    this.orders$.next(this.orders);
  }

  private cookPizza(pizzaOrder: PizzaOrder): void {
    const baseCookingTime = 5;
    let toppingsCookingTime = 0;

    // TODO: Implement logic to sum the cooking time based on the toppings.
    // Put result into toppingsCookingTime

    this.setOrderState(pizzaOrder, PizzaState.cooking);

    setTimeout(() => {
      this.setOrderState(pizzaOrder, PizzaState.ready);
    }, (baseCookingTime + toppingsCookingTime) * 1000);
  }

  private deliverPizza(pizzaOrder: PizzaOrder, driverId: string): void {
    // TODO: implement functionality to set the driver state
  }

  private getTopping(id: number): PizzaTopping {
    return this.toppings.find(x => x.id === id);
  }
}
