git clone https://github.com/BlackSkullGTO/osmlifeInitializationModule.git source
dotnet publish ./source/InitializeActorModule -c release -o ./source/release
cp -f ./source/release/InitializeActorModule.dll .
rm -rf ./source
