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

%/texture_async_apply.dll: src~/texture_async_apply.cpp | %
	$(CXX) -o $@ $^ $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/libtexture_async_apply.so: CXXFLAGS += -fPIC
%/libtexture_async_apply.so: src~/texture_async_apply.cpp | %
	$(CXX) -o $@ $^ $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/libtexture_async_apply.dylib: src~/texture_async_apply.cpp | %
	$(CXX) -o $@ $^ $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/texture_async_apply.o: src~/texture_async_apply.cpp | %
	$(CXX) -c -o $@ $^ $(CXXFLAGS)

%/libtexture_async_apply.a: %/texture_async_apply.o | %
	$(AR) r $@ $<
	$(STRIP) -x $@

# Linux
build/Linux/x86/libtexture_async_apply.so: CXXFLAGS += -m32

# macOS
build/macOS/libtexture_async_apply.dylib: CXXFLAGS += -arch x86_64 -arch arm64

# iOS
build/iOS/libtexture_async_apply.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk iphoneos)

# tvOS
build/tvOS/libtexture_async_apply.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk appletvos)

# visionOS
build/visionOS/libtexture_async_apply.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk xros)

# Android
build/Android/%/libtexture_async_apply.so: CXXFLAGS += -static-libstdc++
build/Android/%/libtexture_async_apply.so: STRIP = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/llvm-strip)

build/Android/arm64/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang++)
build/Android/arm32/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang++)
build/Android/x86_64/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang++)
build/Android/x86/libtexture_async_apply.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android19-clang++)

# WebGL
build/WebGL/texture_async_apply.bc: src~/texture_async_apply.cpp | build/WebGL
	$(EMCXX) -o $@ -c $< $(CXXFLAGS) -emit-llvm

# Targets
windows-x86_64: build/Windows/x86_64/texture_async_apply.dll
windows-x86: build/Windows/x86/texture_async_apply.dll
all-windows: windows-x86_64 windows-x86

windows-mingw-x86_64: CXX = x86_64-w64-mingw32-c++
windows-mingw-x86_64: STRIP = x86_64-w64-mingw32-strip
windows-mingw-x86_64: LINKFLAGS += -static-libgcc -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic
windows-mingw-x86_64: build/Windows/x86_64/texture_async_apply.dll

windows-mingw-x86: CXX = i686-w64-mingw32-c++
windows-mingw-x86: STRIP = i686-w64-mingw32-strip
windows-mingw-x86: LINKFLAGS += -static-libgcc -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic
windows-mingw-x86: build/Windows/x86/texture_async_apply.dll
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86

linux-x86_64: build/Linux/x86_64/libtexture_async_apply.so
all-linux: linux-x86_64

macos-universal: build/macOS/libtexture_async_apply.dylib
ios-arm64: build/iOS/libtexture_async_apply.a
tvos-arm64: build/tvOS/libtexture_async_apply.a
visionos-arm64: build/visionOS/libtexture_async_apply.a
all-apple: macos-universal ios-arm64 tvos-arm64 visionos-arm64

android-arm64: build/Android/arm64/libtexture_async_apply.so
android-arm32: build/Android/arm32/libtexture_async_apply.so
android-x86_64: build/Android/x86_64/libtexture_async_apply.so
android-x86: build/Android/x86/libtexture_async_apply.so
all-android: android-arm64 android-arm32 android-x86_64 android-x86

webgl: build/WebGL/texture_async_apply.bc
all-webgl: webgl