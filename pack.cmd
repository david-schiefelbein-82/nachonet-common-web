@setlocal
@set VER=8.0.0.6

pushd Nachonet.Common.Web
dotnet pack /p:AssemblyVersion=%VER% /p:Version=%VER%
move ".\bin\Release\Nachonet.Common.Web.%VER%.nupkg" "%DEV_PACKAGE_DIR%"
popd

pushd Nachonet.Common.Web.Oidc
dotnet pack /p:AssemblyVersion=%VER% /p:Version=%VER%
move ".\bin\Release\Nachonet.Common.Web.Oidc.%VER%.nupkg" "%DEV_PACKAGE_DIR%"
popd

pushd Nachonet.Common.Web.ActiveDirectory
dotnet pack /p:AssemblyVersion=%VER% /p:Version=%VER%
move ".\bin\Release\Nachonet.Common.Web.ActiveDirectory.%VER%.nupkg" "%DEV_PACKAGE_DIR%"
popd

pushd Nachonet.Common.Web.AppLocal
dotnet pack /p:AssemblyVersion=%VER% /p:Version=%VER%
move ".\bin\Release\Nachonet.Common.Web.AppLocal.%VER%.nupkg" "%DEV_PACKAGE_DIR%"
popd

@endlocal