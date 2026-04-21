import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ShipmentService, Shipment } from './services/shipment.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('Logistics Delay Prediction System');
  protected shipments = signal<Shipment[]>([]);
  protected loading = signal(false);
  protected error = signal<string | null>(null);

  constructor(private shipmentService: ShipmentService) {}

  ngOnInit() {
    this.loadShipments();
  }

  loadShipments() {
    this.loading.set(true);
    this.error.set(null);
    this.shipmentService.getAllShipments().subscribe({
      next: (data) => {
        this.shipments.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading shipments:', err);
        this.error.set('Failed to load shipments. Make sure the backend is running on http://localhost:5075');
        this.loading.set(false);
      }
    });
  }
}
