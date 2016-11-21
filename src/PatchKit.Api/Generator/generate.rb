require 'json'
require_relative 'model_generator.rb'
require_relative 'requests_generator.rb'

DATA = JSON.parse(File.read("swagger.json"))

DATA["definitions"].each do |model_name, model|
  generator = ModelGenerator.new(model_name, model)
  File.open("Models/#{model_name}.cs", "w") do |output|
    output.write generator.generate
  end
end

File.open("ApiConnection.Generated.cs", "w") do |output|
  generator = RequestsGenerator.new(DATA)
  output.write generator.generate
end
