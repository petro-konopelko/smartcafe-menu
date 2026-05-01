# Changelog

## [2.0.0](https://github.com/petro-konopelko/smartcafe-menu/compare/v1.0.0...v2.0.0) (2026-05-01)


### ⚠ BREAKING CHANGES

* update version to v1
* 
* 
* 

### Features

* add cafe endpoints and related functionality ([#83](https://github.com/petro-konopelko/smartcafe-menu/issues/83)) ([0cb2179](https://github.com/petro-konopelko/smartcafe-menu/commit/0cb2179e334c5ebf66a0f1fb8f43dca9f196c21c))
* add cafe seed ([103cc2e](https://github.com/petro-konopelko/smartcafe-menu/commit/103cc2e63db407320f8dff165d70a36e2e331296))
* add copilot instructions for service generation ([fd1c848](https://github.com/petro-konopelko/smartcafe-menu/commit/fd1c84874d871fb372c027278bef1ddf2323d5bd))
* add CORS support to Menu API from admin client app ([5afe53e](https://github.com/petro-konopelko/smartcafe-menu/commit/5afe53ea6a45010c4de42ebfbf0e5b8bb39a7dc3))
* add database migrator for local environment ([c587cb4](https://github.com/petro-konopelko/smartcafe-menu/commit/c587cb4bf737c80054b4414e079c538b65c736fe))
* add menu service ([583d41c](https://github.com/petro-konopelko/smartcafe-menu/commit/583d41cb1f738f98d135ee6ec202b7085a21b97a))
* add persistence volume for local db ([0429fe6](https://github.com/petro-konopelko/smartcafe-menu/commit/0429fe6a741c7d854f8ab0ffde2e15cc2cc95206))
* **build:** add cd deployment pipeline ([606f2e2](https://github.com/petro-konopelko/smartcafe-menu/commit/606f2e22a56ce43357a2d4ea15268dee31859c9b))
* update version to v1 ([e5f03b3](https://github.com/petro-konopelko/smartcafe-menu/commit/e5f03b3f4bb1226524ca0ecea6d01b043628319d))


### Bug Fixes

* incorrect generic type in ValidationBehavior registration ([e05dcf4](https://github.com/petro-konopelko/smartcafe-menu/commit/e05dcf4e81d04f7e24dcc39bf32d14e086bddd2e))
* remove the id from update validator, change discount to percent ([e0714b1](https://github.com/petro-konopelko/smartcafe-menu/commit/e0714b12153c8cf58731b73af231277ebd5b0e17))
* update Aspire configuration, stabilize app ([b42f2d9](https://github.com/petro-konopelko/smartcafe-menu/commit/b42f2d95d2fbaf03123c27937c54634361c02e24))


### Code Refactoring

* Store original and thumbnail image paths instead of full and cropped versions. ([8dee76f](https://github.com/petro-konopelko/smartcafe-menu/commit/8dee76fc10d51c3bd02eb0623814b7fb252f3b5d))
* unify endpoints to use shared menu models ([d90d5ae](https://github.com/petro-konopelko/smartcafe-menu/commit/d90d5ae3fbe9aca725cd4b29e00b03b0b4bd932f))
* update DDD logic to create entities, move shared usages to the new project ([927bc22](https://github.com/petro-konopelko/smartcafe-menu/commit/927bc227f4e43daf77ebcae200eec9cd32a3af65))
