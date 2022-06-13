.PHONY: compiler website

all: compiler website
 
compiler:
	cd ./compiler; \
	dotnet build --nologo

website:
	cd ./website; \
	bundle exec jekyll build