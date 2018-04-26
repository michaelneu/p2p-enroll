all: restore build

build: P2P.Enroll/bin/Debug/netcoreapp2.0/Be.IO.dll
	./dotnet build

P2P.Enroll/bin/Debug/netcoreapp2.0/Be.IO.dll:
	./dotnet restore --packages "P2P.Enroll/lib"

	@mkdir -p "P2P.Enroll/bin/Debug/netcoreapp2.0/"
	@cp "P2P.Enroll/lib/be.io-netstandard/1.0.3/lib/netstandard1.0/Be.IO.dll" "P2P.Enroll/bin/Debug/netcoreapp2.0/"

clean:
	rm -rf "P2P.Enroll/lib" "P2P.Enroll/bin" "P2P.Enroll/obj"

run: 
	./dotnet P2P.Enroll/bin/Debug/netcoreapp2.0/P2P.Enroll.dll