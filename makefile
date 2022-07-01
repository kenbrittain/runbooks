.PHONY: compiler web

all: compiler web
 
compiler:
	cd ./compiler; \
	dotnet build --nologo

web:
	cd ./docs; \
	bundle exec jekyll build
