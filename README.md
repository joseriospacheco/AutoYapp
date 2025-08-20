# 🚗 Sistema de Alquiler de Vehículos en C# (Programación Estructurada)

Este proyecto es un **sistema de alquiler de vehículos en consola** desarrollado en **C#** usando **programación estructurada** (sin POO).  
Permite gestionar vehículos, clientes y alquileres, calcular los cobros de uso y mora, y exportar la información a CSV.

---

## 🚀 Características

- **Vehículos**
  - Registro de vehículos con placa, marca, modelo, año y tarifa diaria.
  - Control de disponibilidad según alquiler y devolución.
  - Listado de vehículos.

- **Clientes**
  - Registro de clientes con nombre, documento y teléfono.
  - Listado de clientes.

- **Alquileres**
  - Creación de alquileres asignando cliente y vehículo.
  - Registro de fecha inicio y fecha fin prevista.
  - Registro de devoluciones (libera vehículo).
  - Cálculo de cobro según reglas:
    - **Tarifa por día.**
    - **Descuento 10%** si el alquiler es de 7 días o más.
    - **Mora**: cada día extra después de la fecha fin prevista cobra **1.5× tarifa diaria**.
  - Listado de alquileres activos e históricos.

- **Exportación**
  - Generación de archivo CSV con toda la información de los alquileres.

---

## ⚙️ Tecnologías utilizadas

- Lenguaje: **C# 10+**
- Framework: **.NET 6 o superior**
- IDE recomendado: **Visual Studio Code** o **Visual Studio 2022**

---

## 📦 Instalación y ejecución

1. Clona este repositorio:

```bash
git clone https://github.com/tu-usuario/alquiler-vehiculos-csharp.git
