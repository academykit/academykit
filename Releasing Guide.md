# Production Release Guide

1. Verify all changes are good
1. Update the `<version></version>` tag in the `AcademyKit.Server/AcademyKit.Server.csproj`
1. Commit to the `main` branch
1. Go to github and create a release with the tag `v`+ the version you have added in the project file above in step 2.
1. Publish the release
1. This will be run github workflow to release, which will push the image with that version tagged to the ecr.
1. Ask client to deploy based on the new tag.

## TODO

- automatic check for updates
