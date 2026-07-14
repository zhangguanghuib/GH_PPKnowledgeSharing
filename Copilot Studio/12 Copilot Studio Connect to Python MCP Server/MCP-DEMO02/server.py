from mcp.server.fastmcp import FastMCP

mcp = FastMCP("demo-mcp", stateless_http=True, auth = None, host="0.0.0.0")

@mcp.tool("add", description="Add two numbers")
def add(a: int, b: int) -> int:
    return a + b

@mcp.tool("minus", description="Subtract two numbers")
def minus(a: int, b: int) -> int:
    return a - b

if __name__ == "__main__":
    mcp.run(transport="streamable-http")