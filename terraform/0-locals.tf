locals {
  num                 = 11
  env                 = "dev"
  region              = "eastus2"
  resource_group_name = "tutorial${local.num}"
  eks_name            = "demo${local.num}"
  eks_version         = "1.27"
  vpc                 = "main${local.num}"
  identity            = "base${local.num}"
  prefix              = "devaks${local.num}"
  dev                 = "dev-test${local.num}"
  acr_name            = "stef${local.num}acr${local.num}"
}
