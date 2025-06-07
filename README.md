# UnityExtensions
[![openupm](https://img.shields.io/npm/v/net.slothsoft.test-runner?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/net.slothsoft.test-runner/)

C# extensions functions and and utility classes for better test-driven-development in Unity.

[Documentation](https://faulo.github.io/UnityExtensions/api/Slothsoft.UnityExtensions.html)

## Key features:
- GameObject.AddSubstitute<T> for adding substitute Components that implement or extend T.
- TestGameObject, a wrapper around GameObject that destroys it on Dispose.
- TestObjectStore, a wrapper around Unity's Object instantiation that destroys all of them on Dispose.
- TimeScaler, a wrapper around Unity's Time.timeScale that resets the scale on Dispose.
- WaitUtils.WaitFor, a coroutine that asserts that something has happened after some time has passed.

## Requirements
- Unity 6000.0.44f1
- net.tnrd.nsubstitute (on OpenUPM)

## Installation
### Install via manifest.json
The package is available on the [openupm registry](https://openupm.com/packages/net.slothsoft.test-runner/). The easiest way to install it is to set up a scoped registry via Unity's manifest.json:
```
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "net.slothsoft.test-runner",
        "net.tnrd.nsubstitute"
      ]
    }
  ],
  "dependencies": {
    "net.slothsoft.test-runner": "1.0.0",
  }
}
```

### Install via OpenUPM-CLI
Alternatively, you may install it via [openupm-cli](https://github.com/openupm/openupm-cli):
```
openupm add net.slothsoft.test-runner
```
