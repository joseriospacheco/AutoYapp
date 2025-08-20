namespace AutoYapp;

using System;
using System.Globalization;
using System.IO;

class Program
{
    // =======================
    // Parámetros de negocio
    // =======================
    const int MAX_VEHICULOS = 300;
    const int MAX_CLIENTES = 500;
    const int MAX_ALQUILERES = 1000;

    // Política de precios
    const decimal MULTA_DIA_MORA_FACTOR = 1.5m; // cada día de mora cobra 1.5x tarifa diaria
    const decimal DESCUENTO_LARGO_ALQUILER = 0.10m; // 10% si >= 7 días (ejemplo)
    const int UMBRAL_DIAS_DESC = 7;

    // =======================
    // "Tablas" (arreglos paralelos)
    // =======================

    // Vehículos
    static int[] vehId = new int[MAX_VEHICULOS];
    static string[] vehPlaca = new string[MAX_VEHICULOS];
    static string[] vehMarca = new string[MAX_VEHICULOS];
    static string[] vehModelo = new string[MAX_VEHICULOS];
    static int[] vehAnio = new int[MAX_VEHICULOS];
    static decimal[] vehTarifaDia = new decimal[MAX_VEHICULOS];
    static bool[] vehDisponible = new bool[MAX_VEHICULOS];
    static int vehCount = 0;
    static int nextVehId = 1;

    // Clientes
    static int[] cliId = new int[MAX_CLIENTES];
    static string[] cliNombre = new string[MAX_CLIENTES];
    static string[] cliDocumento = new string[MAX_CLIENTES];
    static string[] cliTelefono = new string[MAX_CLIENTES];
    static int cliCount = 0;
    static int nextCliId = 1;

    // Alquileres
    static int[] alqId = new int[MAX_ALQUILERES];
    static int[] alqVehId = new int[MAX_ALQUILERES];
    static int[] alqCliId = new int[MAX_ALQUILERES];
    static DateTime[] alqFechaInicio = new DateTime[MAX_ALQUILERES];
    static DateTime[] alqFechaFinPrevista = new DateTime[MAX_ALQUILERES];
    static DateTime[] alqFechaDevolucion = new DateTime[MAX_ALQUILERES]; // DateTime.MinValue si no devuelto
    static decimal[] alqTarifaDia = new decimal[MAX_ALQUILERES];
    static int alqCount = 0;
    static int nextAlqId = 1;

    static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-CO");

        // Datos demo (opcional)
        SeedDemo();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("      SISTEMA ALQUILER DE VEHÍCULOS       ");
            Console.WriteLine("            (Estructurado C#)             ");
            Console.WriteLine("==========================================");
            Console.WriteLine("1) Registrar vehículo");
            Console.WriteLine("2) Listar vehículos");
            Console.WriteLine("3) Registrar cliente");
            Console.WriteLine("4) Listar clientes");
            Console.WriteLine("5) Crear alquiler");
            Console.WriteLine("6) Listar alquileres");
            Console.WriteLine("7) Registrar devolución");
            Console.WriteLine("8) Calcular cobro de un alquiler");
            Console.WriteLine("9) Exportar alquileres a CSV");
            Console.WriteLine("0) Salir");
            Console.Write("Opción: ");
            string op = Console.ReadLine()?.Trim() ?? "";

            switch (op)
            {
                case "1": RegistrarVehiculo(); break;
                case "2": ListarVehiculos(); Pausa(); break;
                case "3": RegistrarCliente(); break;
                case "4": ListarClientes(); Pausa(); break;
                case "5": CrearAlquiler(); break;
                case "6": ListarAlquileres(); Pausa(); break;
                case "7": RegistrarDevolucion(); break;
                case "8": CalcularCobroAlquilerPrompt(); break;
                case "9": ExportarAlquileresCSV(); break;
                case "0": return;
                default:
                    Console.WriteLine("Opción no válida.");
                    Pausa();
                    break;
            }
        }
    }

    // =======================
    // Vehículos
    // =======================
    static void RegistrarVehiculo()
    {
        Console.Clear();
        Console.WriteLine("=== Registrar vehículo ===");
        if (vehCount >= MAX_VEHICULOS) { Console.WriteLine("Capacidad de vehículos llena."); Pausa(); return; }

        Console.Write("Placa: ");
        string placa = (Console.ReadLine() ?? "").Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(placa)) { Console.WriteLine("Placa inválida."); Pausa(); return; }

        // Validar duplicado
        for (int i = 0; i < vehCount; i++)
            if (vehPlaca[i].Equals(placa, StringComparison.OrdinalIgnoreCase))
            { Console.WriteLine("Ya existe un vehículo con esa placa."); Pausa(); return; }

        Console.Write("Marca: ");
        string marca = (Console.ReadLine() ?? "").Trim();
        Console.Write("Modelo: ");
        string modelo = (Console.ReadLine() ?? "").Trim();

        Console.Write("Año: ");
        if (!int.TryParse(Console.ReadLine(), out int anio) || anio < 1980 || anio > DateTime.Now.Year + 1)
        { Console.WriteLine("Año inválido."); Pausa(); return; }

        Console.Write("Tarifa por día ($): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal tarifa) || tarifa <= 0)
        { Console.WriteLine("Tarifa inválida."); Pausa(); return; }

        int p = vehCount;
        vehId[p] = nextVehId++;
        vehPlaca[p] = placa;
        vehMarca[p] = marca;
        vehModelo[p] = modelo;
        vehAnio[p] = anio;
        vehTarifaDia[p] = tarifa;
        vehDisponible[p] = true;
        vehCount++;

        Console.WriteLine($"Vehículo registrado (ID {vehId[p]}).");
        Pausa();
    }

    static void ListarVehiculos()
    {
        Console.Clear();
        Console.WriteLine("=== Vehículos ===");
        if (vehCount == 0) { Console.WriteLine("No hay vehículos."); return; }
        Console.WriteLine($"{"ID",4} | {"Placa",-8} | {"Marca",-12} | {"Modelo",-12} | {"Año",4} | {"Tarifa Día",12} | {"Disp.",5}");
        Console.WriteLine(new string('-', 70));
        for (int i = 0; i < vehCount; i++)
        {
            Console.WriteLine($"{vehId[i],4} | {vehPlaca[i],-8} | {Trunc(vehMarca[i], 12),-12} | {Trunc(vehModelo[i], 12),-12} | {vehAnio[i],4} | {vehTarifaDia[i],12:C0} | {(vehDisponible[i] ? "Sí" : "No"),5}");
        }
    }

    static int BuscarVehiculoPorIdPrompt()
    {
        ListarVehiculos();
        Console.WriteLine();
        Console.Write("ID del vehículo: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID inválido."); Pausa(); return -1; }
        int pos = -1;
        for (int i = 0; i < vehCount; i++) if (vehId[i] == id) { pos = i; break; }
        if (pos == -1) { Console.WriteLine("Vehículo no encontrado."); Pausa(); }
        return pos;
    }

    // =======================
    // Clientes
    // =======================
    static void RegistrarCliente()
    {
        Console.Clear();
        Console.WriteLine("=== Registrar cliente ===");
        if (cliCount >= MAX_CLIENTES) { Console.WriteLine("Capacidad de clientes llena."); Pausa(); return; }

        Console.Write("Nombre completo: ");
        string nombre = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre)) { Console.WriteLine("Nombre inválido."); Pausa(); return; }

        Console.Write("Documento (CC/NIT): ");
        string doc = (Console.ReadLine() ?? "").Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(doc)) { Console.WriteLine("Documento inválido."); Pausa(); return; }

        // (simple) Validar duplicado por documento
        for (int i = 0; i < cliCount; i++)
            if (cliDocumento[i].Equals(doc, StringComparison.OrdinalIgnoreCase))
            { Console.WriteLine("Ya existe un cliente con ese documento."); Pausa(); return; }

        Console.Write("Teléfono: ");
        string tel = (Console.ReadLine() ?? "").Trim();

        int p = cliCount;
        cliId[p] = nextCliId++;
        cliNombre[p] = nombre;
        cliDocumento[p] = doc;
        cliTelefono[p] = tel;
        cliCount++;

        Console.WriteLine($"Cliente registrado (ID {cliId[p]}).");
        Pausa();
    }

    static void ListarClientes()
    {
        Console.Clear();
        Console.WriteLine("=== Clientes ===");
        if (cliCount == 0) { Console.WriteLine("No hay clientes."); return; }
        Console.WriteLine($"{"ID",4} | {"Nombre",-28} | {"Documento",-12} | {"Teléfono",-12}");
        Console.WriteLine(new string('-', 70));
        for (int i = 0; i < cliCount; i++)
        {
            Console.WriteLine($"{cliId[i],4} | {Trunc(cliNombre[i], 28),-28} | {Trunc(cliDocumento[i], 12),-12} | {Trunc(cliTelefono[i], 12),-12}");
        }
    }

    static int BuscarClientePorIdPrompt()
    {
        ListarClientes();
        Console.WriteLine();
        Console.Write("ID del cliente: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID inválido."); Pausa(); return -1; }
        int pos = -1;
        for (int i = 0; i < cliCount; i++) if (cliId[i] == id) { pos = i; break; }
        if (pos == -1) { Console.WriteLine("Cliente no encontrado."); Pausa(); }
        return pos;
    }

    // =======================
    // Alquileres
    // =======================
    static void CrearAlquiler()
    {
        Console.Clear();
        Console.WriteLine("=== Crear alquiler ===");
        if (alqCount >= MAX_ALQUILERES) { Console.WriteLine("Capacidad de alquileres llena."); Pausa(); return; }
        if (vehCount == 0 || cliCount == 0) { Console.WriteLine("Debe haber vehículos y clientes registrados."); Pausa(); return; }

        int posVeh = BuscarVehiculoPorIdPrompt();
        if (posVeh == -1) return;
        if (!vehDisponible[posVeh]) { Console.WriteLine("Vehículo NO disponible."); Pausa(); return; }

        int posCli = BuscarClientePorIdPrompt();
        if (posCli == -1) return;

        Console.Write("Fecha inicio (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime fInicio))
        { Console.WriteLine("Fecha inválida."); Pausa(); return; }

        Console.Write("Fecha fin prevista (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime fFinPrev) || fFinPrev < fInicio)
        { Console.WriteLine("Fecha fin prevista inválida."); Pausa(); return; }

        int idx = alqCount;
        alqId[idx] = nextAlqId++;
        alqVehId[idx] = vehId[posVeh];
        alqCliId[idx] = cliId[posCli];
        alqFechaInicio[idx] = fInicio.Date;
        alqFechaFinPrevista[idx] = fFinPrev.Date;
        alqFechaDevolucion[idx] = DateTime.MinValue; // aún no devuelto
        alqTarifaDia[idx] = vehTarifaDia[posVeh];
        alqCount++;

        vehDisponible[posVeh] = false;

        Console.WriteLine($"Alquiler creado (ID {alqId[idx]}).");
        Pausa();
    }

    static void ListarAlquileres()
    {
        Console.Clear();
        Console.WriteLine("=== Alquileres ===");
        if (alqCount == 0) { Console.WriteLine("No hay alquileres."); return; }
        Console.WriteLine($"{"ID",4} | {"Veh(ID/Placa)",-14} | {"Cliente(ID/Doc)",-18} | {"Inicio",-10} | {"FinPrev",-10} | {"Devuelto",-10} | {"Tarifa",10}");
        Console.WriteLine(new string('-', 100));
        for (int i = 0; i < alqCount; i++)
        {
            var v = GetVehIndexById(alqVehId[i]);
            var c = GetCliIndexById(alqCliId[i]);
            string placa = v >= 0 ? vehPlaca[v] : "?";
            string doc = c >= 0 ? cliDocumento[c] : "?";
            string dev = alqFechaDevolucion[i] == DateTime.MinValue ? "-" : alqFechaDevolucion[i].ToString("yyyy-MM-dd");
            Console.WriteLine($"{alqId[i],4} | {alqVehId[i]}/{placa,-11} | {alqCliId[i]}/{doc,-15} | {alqFechaInicio[i]:yyyy-MM-dd} | {alqFechaFinPrevista[i]:yyyy-MM-dd} | {dev,-10} | {alqTarifaDia[i],10:C0}");
        }
    }

    static void RegistrarDevolucion()
    {
        Console.Clear();
        Console.WriteLine("=== Registrar devolución ===");
        if (alqCount == 0) { Console.WriteLine("No hay alquileres."); Pausa(); return; }

        int posAlq = BuscarAlquilerPorIdPrompt();
        if (posAlq == -1) return;

        if (alqFechaDevolucion[posAlq] != DateTime.MinValue)
        { Console.WriteLine("Este alquiler ya fue devuelto."); Pausa(); return; }

        Console.Write("Fecha de devolución (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime fDev))
        { Console.WriteLine("Fecha inválida."); Pausa(); return; }

        if (fDev < alqFechaInicio[posAlq])
        { Console.WriteLine("La devolución no puede ser antes del inicio."); Pausa(); return; }

        alqFechaDevolucion[posAlq] = fDev.Date;

        // Marcar vehículo disponible
        int v = GetVehIndexById(alqVehId[posAlq]);
        if (v >= 0) vehDisponible[v] = true;

        Console.WriteLine("Devolución registrada.");
        // Mostrar cobro
        CalcularCobroYMostrar(posAlq);
        Pausa();
    }

    static void CalcularCobroAlquilerPrompt()
    {
        Console.Clear();
        Console.WriteLine("=== Calcular cobro de un alquiler ===");
        if (alqCount == 0) { Console.WriteLine("No hay alquileres."); Pausa(); return; }
        int posAlq = BuscarAlquilerPorIdPrompt();
        if (posAlq == -1) return;
        CalcularCobroYMostrar(posAlq);
        Pausa();
    }

    static void CalcularCobroYMostrar(int posAlq)
    {
        CalcularCobro(posAlq,
            out int diasPrevistos,
            out int diasCobrados,
            out int diasMora,
            out decimal subtotalDias,
            out decimal descuento,
            out decimal valorMora,
            out decimal total);

        var v = GetVehIndexById(alqVehId[posAlq]);
        var c = GetCliIndexById(alqCliId[posAlq]);
        string placa = v >= 0 ? vehPlaca[v] : "?";
        string nombre = c >= 0 ? cliNombre[c] : "?";

        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"Alquiler ID: {alqId[posAlq]}");
        Console.WriteLine($"Cliente: {nombre} (ID {alqCliId[posAlq]})");
        Console.WriteLine($"Vehículo: {placa} (ID {alqVehId[posAlq]})");
        Console.WriteLine($"Inicio: {alqFechaInicio[posAlq]:yyyy-MM-dd} | Fin previsto: {alqFechaFinPrevista[posAlq]:yyyy-MM-dd}");
        string dev = alqFechaDevolucion[posAlq] == DateTime.MinValue ? "(no devuelto)" : alqFechaDevolucion[posAlq].ToString("yyyy-MM-dd");
        Console.WriteLine($"Devolución: {dev}");
        Console.WriteLine($"Tarifa/día: {alqTarifaDia[posAlq]:C0}");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Días previstos: {diasPrevistos}");
        Console.WriteLine($"Días cobrados: {diasCobrados}");
        Console.WriteLine($"Días en mora: {diasMora}");
        Console.WriteLine($"Subtotal días: {subtotalDias:C0}");
        Console.WriteLine($"Descuento (≥{UMBRAL_DIAS_DESC} días): -{descuento:C0}");
        Console.WriteLine($"Mora: +{valorMora:C0}");
        Console.WriteLine($"TOTAL: {total:C0}");
    }

    // Regla de cobro:
    // - Días cobrados = al menos 1 día entre inicio y fecha de devolución (o hoy si no devuelto).
    // - Descuento 10% si diasCobrados >= 7.
    // - Mora: si se devuelve después de la fecha fin prevista, cada día extra cuesta 1.5x la tarifa diaria.
    static void CalcularCobro(
        int posAlq,
        out int diasPrevistos,
        out int diasCobrados,
        out int diasMora,
        out decimal subtotalDias,
        out decimal descuento,
        out decimal valorMora,
        out decimal total)
    {
        DateTime hoy = DateTime.Today;
        DateTime finReal = (alqFechaDevolucion[posAlq] == DateTime.MinValue) ? hoy : alqFechaDevolucion[posAlq];

        diasPrevistos = Math.Max(1, (alqFechaFinPrevista[posAlq] - alqFechaInicio[posAlq]).Days + 1); // inclusivo
        diasCobrados = Math.Max(1, (finReal - alqFechaInicio[posAlq]).Days + 1); // inclusivo
        diasMora = Math.Max(0, (finReal - alqFechaFinPrevista[posAlq]).Days);

        decimal tarifa = alqTarifaDia[posAlq];
        subtotalDias = RedondearMoneda(diasCobrados * tarifa);

        descuento = 0;
        if (diasCobrados >= UMBRAL_DIAS_DESC)
            descuento = RedondearMoneda(subtotalDias * DESCUENTO_LARGO_ALQUILER);

        valorMora = RedondearMoneda(diasMora * tarifa * MULTA_DIA_MORA_FACTOR);

        total = RedondearMoneda(subtotalDias - descuento + valorMora);
    }

    // =======================
    // Exportación
    // =======================
    static void ExportarAlquileresCSV()
    {
        Console.Clear();
        Console.WriteLine("=== Exportar alquileres a CSV ===");
        if (alqCount == 0) { Console.WriteLine("No hay alquileres."); Pausa(); return; }

        Console.Write("Nombre del archivo (ej. alquileres.csv): ");
        string nombre = (Console.ReadLine() ?? "alquileres.csv").Trim();
        if (!nombre.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            nombre += ".csv";

        using (var sw = new StreamWriter(nombre))
        {
            sw.WriteLine("AlquilerID,ClienteID,Cliente,Documento,VehiculoID,Placa,FechaInicio,FechaFinPrevista,FechaDevolucion,TarifaDia,DiasPrevistos,DiasCobrados,DiasMora,Subtotal,Descuento,Mora,Total");

            for (int i = 0; i < alqCount; i++)
            {
                CalcularCobro(i, out int dp, out int dc, out int dm, out decimal sub, out decimal desc, out decimal mora, out decimal total);

                int vi = GetVehIndexById(alqVehId[i]);
                int ci = GetCliIndexById(alqCliId[i]);

                string placa = vi >= 0 ? vehPlaca[vi] : "";
                string cliente = ci >= 0 ? cliNombre[ci] : "";
                string doc = ci >= 0 ? cliDocumento[ci] : "";
                string fdev = alqFechaDevolucion[i] == DateTime.MinValue ? "" : alqFechaDevolucion[i].ToString("yyyy-MM-dd");

                sw.WriteLine(string.Join(",",
                    alqId[i],
                    alqCliId[i],
                    EscaparCSV(cliente),
                    EscaparCSV(doc),
                    alqVehId[i],
                    EscaparCSV(placa),
                    alqFechaInicio[i].ToString("yyyy-MM-dd"),
                    alqFechaFinPrevista[i].ToString("yyyy-MM-dd"),
                    fdev,
                    alqTarifaDia[i].ToString("0"),
                    dp, dc, dm,
                    sub.ToString("0"),
                    desc.ToString("0"),
                    mora.ToString("0"),
                    total.ToString("0")
                ));
            }
        }

        Console.WriteLine($"Archivo generado: {nombre}");
        Pausa();
    }

    // =======================
    // Utilidades
    // =======================
    static int BuscarAlquilerPorIdPrompt()
    {
        ListarAlquileres();
        Console.WriteLine();
        Console.Write("ID del alquiler: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID inválido."); Pausa(); return -1; }
        int pos = -1;
        for (int i = 0; i < alqCount; i++) if (alqId[i] == id) { pos = i; break; }
        if (pos == -1) { Console.WriteLine("Alquiler no encontrado."); Pausa(); }
        return pos;
    }

    static int GetVehIndexById(int id)
    {
        for (int i = 0; i < vehCount; i++) if (vehId[i] == id) return i;
        return -1;
    }

    static int GetCliIndexById(int id)
    {
        for (int i = 0; i < cliCount; i++) if (cliId[i] == id) return i;
        return -1;
    }

    static string Trunc(string s, int len) => string.IsNullOrEmpty(s) ? s : (s.Length <= len ? s : s.Substring(0, len - 1) + "…");
    static string EscaparCSV(string s) => (s.Contains(",") || s.Contains("\"")) ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
    static decimal RedondearMoneda(decimal v) => Math.Round(v, 0, MidpointRounding.AwayFromZero);

    static void Pausa() { Console.WriteLine(); Console.Write("Continuar... "); Console.ReadKey(); }

    static void SeedDemo()
    {
        // Vehículos demo
        vehId[vehCount] = nextVehId++; vehPlaca[vehCount] = "ABC123"; vehMarca[vehCount] = "Chevrolet"; vehModelo[vehCount] = "Onix"; vehAnio[vehCount] = 2022; vehTarifaDia[vehCount] = 120000; vehDisponible[vehCount] = true; vehCount++;
        vehId[vehCount] = nextVehId++; vehPlaca[vehCount] = "XYZ987"; vehMarca[vehCount] = "Renault"; vehModelo[vehCount] = "Duster"; vehAnio[vehCount] = 2021; vehTarifaDia[vehCount] = 180000; vehDisponible[vehCount] = true; vehCount++;
        // Clientes demo
        cliId[cliCount] = nextCliId++; cliNombre[cliCount] = "María Gómez"; cliDocumento[cliCount] = "CC123"; cliTelefono[cliCount] = "3001112222"; cliCount++;
        cliId[cliCount] = nextCliId++; cliNombre[cliCount] = "Juan Pérez"; cliDocumento[cliCount] = "CC456"; cliTelefono[cliCount] = "3003334444"; cliCount++;
    }
}
