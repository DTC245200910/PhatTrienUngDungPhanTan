var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dữ liệu mẫu lưu trên bộ nhớ RAM
var products = new List<ProductDto>
{
    new(1, "Laptop Dell XPS 13", "May tinh xach tay mong nhe", 25000000m),
    new(2, "Chuot Logitech MX Master 3S", "Chuot khong day van phong", 2200000m)
};
var nextId = 3;

// 1. GET danh sách sản phẩm
app.MapGet("/api/products", () => Results.Ok(products));

// 2. GET chi tiết sản phẩm theo ID
app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null
        ? Results.NotFound(new ApiError("PRODUCT_NOT_FOUND", $"Khong tim thay san pham co id = {id}."))
        : Results.Ok(product);
});

// 3. POST tạo sản phẩm mới (có Validation và Trim)
app.MapPost("/api/products", (CreateProductRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new ApiError("INVALID_NAME", "Ten san pham khong duoc de trong."));
    }

    if (request.Price <= 0)
    {
        return Results.BadRequest(new ApiError("INVALID_PRICE", "Gia san pham phai lon hon 0."));
    }

    var newProduct = new ProductDto(
        Id: nextId++,
        Name: request.Name.Trim(),
        Description: request.Description?.Trim() ?? string.Empty,
        Price: request.Price
    );

    products.Add(newProduct);

    return Results.Created($"/api/products/{newProduct.Id}", newProduct);
});

app.Run();

// Các DTO theo đúng cấu trúc trong giáo trình
record ProductDto(int Id, string Name, string Description, decimal Price);
record CreateProductRequest(string? Name, string? Description, decimal Price);
record ApiError(string Code, string Message);