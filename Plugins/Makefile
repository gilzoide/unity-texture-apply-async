STRIP ?= strip
EMCXX ?= em++
SED ?= sed
AR ?= ar

ANDROID_NDK_ROOT ?=

LINKFLAGS += -shared
CXXFLAGS += -std=c++11
ifeq ($(DEBUG),1)
	CXXFLAGS += -O0 -g
else
	CXXFLAGS += -O2
endif

BUILD_DIRS = \
	build/Windows/x86_64 build/Windows/x86 \
	build/Linux/x86_64 \
	build/macOS build/iOS build/tvOS build/visionOS \
	build/Android/arm64 build/Android/arm32 build/Android/x86 build/Android/x86_64 \
	build/WebGL

# Misc
$(BUILD_DIRS):
	mkdir -p $@

%/texture_async_apply.dll: src/texture_async_apply.cpp | %
	$(CXX) -o $@ $< $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/libtexture_async_apply.so: CXXFLAGS += -fPIC
%/libtexture_async_apply.so: src/texture_async_apply.cpp | %
	$(CXX) -o $@ $< $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/libtexture_async_apply.dylib: src/texture_async_apply.cpp | %
	$(CXX) -o $@ $< $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/texture_async_apply.o: src/texture_async_apply.cpp | %
	$(CXX) -c -o $@ $< $(CXXFLAGS)

%/libtexture_async_apply.a: %/texture_async_apply.o | %
	$(AR) r $@ $<
	$(STRIP) -x $@

# Linux
build/Linux/x86/libtexture_async_apply.so: CXXFLAGS += -m32

# macOS
build/macOS/libtexture_async_apply.dylib: CXXFLAGS += -arch x86_64 -arch arm64

# Android
check-ndk-root:
ifndef ANDROID_NDK_ROOT
	$(error ANDROID_NDK_ROOT must be set for Android builds!)
endif

build/Android/%/libtexture_async_apply.so: CXXFLAGS += -static-libstdc++
build/Android/%/libtexture_async_apply.so: STRIP = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/llvm-strip)

build/Android/arm64/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang++)
build/Android/arm64/libtexture_async_apply.so: check-ndk-root
build/Android/arm32/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi21-clang++)
build/Android/arm32/libtexture_async_apply.so: check-ndk-root
build/Android/x86_64/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang++)
build/Android/x86_64/libtexture_async_apply.so: check-ndk-root
build/Android/x86/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android21-clang++)
build/Android/x86/libtexture_async_apply.so: check-ndk-root

# WebGL
build/WebGL/texture_async_apply.bc: src/texture_async_apply.cpp | build/WebGL
	$(EMCXX) -o $@ -c $< $(CXXFLAGS) -emit-llvm

# Targets
windows-x86_64: build/Windows/x86_64/texture_async_apply.dll
windows-x86: build/Windows/x86/texture_async_apply.dll
all-windows: windows-x86_64 windows-x86

windows-mingw-x86_64: CXX = x86_64-w64-mingw32-c++
windows-mingw-x86_64: STRIP = x86_64-w64-mingw32-strip
windows-mingw-x86_64: LINKFLAGS += -static-libgcc -static-libstdc++ -static
windows-mingw-x86_64: build/Windows/x86_64/texture_async_apply.dll

windows-mingw-x86: CXX = i686-w64-mingw32-c++
windows-mingw-x86: STRIP = i686-w64-mingw32-strip
windows-mingw-x86: LINKFLAGS += -static-libgcc -static-libstdc++ -static
windows-mingw-x86: build/Windows/x86/texture_async_apply.dll
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86

linux-x86_64: build/Linux/x86_64/libtexture_async_apply.so
all-linux: linux-x86_64

macos-universal: build/macOS/libtexture_async_apply.dylib
all-apple: macos-universal

android-arm64: build/Android/arm64/libtexture_async_apply.so
android-arm32: build/Android/arm32/libtexture_async_apply.so
android-x86_64: build/Android/x86_64/libtexture_async_apply.so
android-x86: build/Android/x86/libtexture_async_apply.so
all-android: android-arm64 android-arm32 android-x86_64 android-x86

webgl: build/WebGL/texture_async_apply.bc
all-webgl: webgl

# Dockerized builds
docker-all-android:
	docker build -f tools~/Dockerfile.build.android -t gilzoide-texture-async-apply-build-android:latest .
	docker run --rm -v "$(CURDIR)":/src -w /src --platform=linux/amd64 gilzoide-texture-async-apply-build-android:latest make all-android
docker-all-linux:
	docker build -f tools~/Dockerfile.build.linux -t gilzoide-texture-async-apply-build-linux:latest .
	docker run --rm -v "$(CURDIR)":/src -w /src --platform=linux/amd64 gilzoide-texture-async-apply-build-linux:latest make all-linux
docker-all-webgl:
	docker build -f tools~/Dockerfile.build.webgl -t gilzoide-texture-async-apply-build-webgl:latest .
	docker run --rm -v "$(CURDIR)":/src -w /src gilzoide-texture-async-apply-build-webgl:latest make all-webgl
docker-all-windows:
	docker build -f tools~/Dockerfile.build.windows -t gilzoide-texture-async-apply-build-windows:latest .
	docker run --rm -v "$(CURDIR)":/src -w /src gilzoide-texture-async-apply-build-windows:latest make all-windows-mingw
