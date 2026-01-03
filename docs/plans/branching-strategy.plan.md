# Branching Strategy Migration Plan

This document outlines the migration from the current dual-branch model to a production-ready branching strategy with `main` as the primary development branch.

## Current State

**Branch structure:**

- `main` - v2 stable sources (legacy default branch)
- `v3` - v3 development (temporary default for convenience)
- `v2` - Does not exist yet

**Deployment configuration:**

- v2 releases: Deploy from `main` branch to NuGet.org (stable releases)
- v3 previews: Deploy from `v3` branch to NuGet.org (preview releases)

**Problem**: Once v3 becomes stable, the branch structure doesn't align with standard Git conventions where `main` should be the primary development branch.

## Target State

**Branch structure:**

- `main` - v3 stable sources (new primary development branch)
- `v3` - **DELETED** (merged into main, no longer needed)
- `v2` - v2 maintenance sources (legacy support)

**Deployment configuration:**

- v2 patches: Deploy from `v2` branch to NuGet.org (maintenance releases only)
- v3 stable: Deploy from `main` branch to NuGet.org (stable releases)
- v3 previews: Deploy from `main` branch to NuGet.org (preview releases until v3.0.0)

**GitHub repository settings:**

- Default branch: `main` (changed from `v3`)
- Protected branches: `main`, `v2`
- PR targets: Default to `main`

## Migration Phases

### Phase 0: Pre-Migration Validation (REQUIRED)

**Must complete BEFORE starting migration:**

- [ ] v3.0.0 stable release ready (or explicitly planned for post-migration release)
- [ ] All critical v3 issues resolved (#1585, community feedback)
- [ ] All CI/CD workflows passing on `v3` branch
- [ ] No active PRs targeting `main` branch
- [ ] Team consensus on migration timing
- [ ] Backup plan documented (rollback steps)

**Estimated time**: N/A (gates only, no work)

### Phase 1: CI/CD Configuration Updates

**Objective**: Update deployment workflows to support dual-branch deployment model.

**Changes required:**

1. **GitHub Actions workflow updates**:
   - [ ] Update `publish.yml` (or equivalent NuGet publish workflow):
     - Add branch condition: Deploy from `v2` branch with v2.x version tags
     - Add branch condition: Deploy from `main` branch with v3.x version tags
     - Verify version detection logic works for both branches
   - [ ] Update `ci.yml` (or equivalent CI workflow):
     - Ensure CI runs on `main` and `v2` branches
     - Update branch protection rules for `main`
   - [ ] Update any other workflows with branch-specific logic

2. **Version detection**:
   - [ ] Verify GitVersion (or versioning tool) correctly detects version from branch
   - [ ] Test version calculation on local `v2` branch (simulated)
   - [ ] Test version calculation on local `main` branch (simulated)

3. **NuGet package validation**:
   - [ ] Verify package metadata for v2.x releases (from `v2` branch)
   - [ ] Verify package metadata for v3.x releases (from `main` branch)
   - [ ] Confirm no version conflicts in NuGet feed

**Validation criteria:**

- [ ] CI passes on `v3` branch with updated workflows
- [ ] Dry-run deployment from simulated `v2` branch succeeds
- [ ] Dry-run deployment from simulated `main` branch succeeds
- [ ] Version numbers calculated correctly for both scenarios

**Estimated time**: 4-6 hours

### Phase 2: Create v2 Maintenance Branch

**Objective**: Preserve current `main` (v2 sources) as `v2` branch for future maintenance.

**Steps:**

1. **Create and verify v2 branch**:

   ```bash
   # From latest main branch
   git checkout main
   git pull origin main
   git checkout -b v2
   git push origin v2
   ```

2. **Validate v2 branch**:
   - [ ] CI workflow runs successfully on `v2` branch
   - [ ] Build succeeds with v2.x version number
   - [ ] All tests pass
   - [ ] Documentation builds successfully

3. **Update v2 branch documentation**:
   - [ ] Add `README.md` notice: "This is the v2 maintenance branch. For v3 development, see `main` branch."
   - [ ] Update contributing docs to direct new work to `main`

**Validation criteria:**

- [ ] `v2` branch exists and mirrors current `main`
- [ ] CI passes on `v2` branch
- [ ] Version detection shows v2.x.x version
- [ ] Branch is protected in GitHub settings

**Estimated time**: 1-2 hours

### Phase 3: Merge v3 into main

**Objective**: Merge `v3` branch into `main` branch using standard merge workflow.

**Note**: PR #1014 (v3 → main) shows clean merge state with no conflicts, so standard merge is preferred over force-push to preserve complete history.

**Steps:**

1. **Prepare for migration**:
   - [ ] Announce migration window to team/community
   - [ ] Document current `main` commit SHA for reference: `___________________`
   - [ ] Document current `v3` commit SHA for reference: `___________________`
   - [ ] Close or retarget any open PRs pointing to `main` (except #1014)
   - [ ] Wait for all in-flight CI jobs to complete
   - [ ] Ensure PR #1014 is ready (all checks passing, up-to-date with both branches)

2. **Execute migration via PR merge**:
   - [ ] **Option A - GitHub UI merge** (recommended for visibility):
     - Navigate to PR #1014
     - Select "Merge pull request" (use merge commit, not squash/rebase)
     - Confirm merge
   - [ ] **Option B - Command line merge** (if PR merge not preferred):

     ```bash
     # Fetch latest changes
     git fetch origin
     
     # Checkout main and ensure it's up-to-date
     git checkout main
     git pull origin main
     
     # Merge v3 into main
     git merge origin/v3 --no-ff -m "feat: Merge v3.0.0 streaming indicators into main"
     
     # Push merged main
     git push origin main
     ```

3. **Verify migration**:
   - [ ] `main` branch contains all v3 sources
   - [ ] Merge commit exists preserving both branch histories
   - [ ] CI runs successfully on new `main`
   - [ ] Version detection shows v3.x.x version
   - [ ] All tests pass
   - [ ] Documentation builds successfully

**Validation criteria:**

- [ ] `main` branch contains v3 sources
- [ ] Complete commit history preserved (both v2 and v3 lineages visible)
- [ ] CI passes on new `main`
- [ ] No broken links or references
- [ ] PR #1014 shows as merged

**Estimated time**: 1-2 hours (including validation)

**Rollback procedure** (if needed within 24 hours):

```bash
# Reset main to pre-migration state using documented SHA
git checkout main
git reset --hard <documented-main-sha>
git push origin main --force
```

### Phase 4: Update Repository Settings

**Objective**: Configure GitHub repository to use `main` as default branch.

**GitHub repository settings:**

1. **Change default branch**:
   - [ ] Navigate to Settings → Branches
   - [ ] Change default branch from `v3` to `main`
   - [ ] Update branch protection rules:
     - `main`: Require PR reviews, passing CI, no force-push
     - `v2`: Require PR reviews, passing CI, no force-push

2. **Update repository description**:
   - [ ] Ensure description reflects v3 as primary version
   - [ ] Update topics/tags if needed

3. **Update documentation references**:
   - [ ] Search for hardcoded `/v3/` branch references in docs
   - [ ] Replace with `/main/` or use dynamic branch references
   - [ ] Update contributor documentation
   - [ ] Update PR template if it references `v3` branch

**Validation criteria:**

- [ ] Default branch is `main`
- [ ] New PRs default to `main` branch
- [ ] Branch protection rules active
- [ ] Documentation links work correctly

**Estimated time**: 1-2 hours

### Phase 5: Delete v3 Branch

**Objective**: Clean up obsolete `v3` branch now that `main` serves as primary.

**Prerequisites:**

- [ ] Phase 3 completed successfully
- [ ] Phase 4 completed successfully  
- [ ] No open PRs targeting `v3` branch
- [ ] At least 48 hours elapsed since migration (safety window)
- [ ] Team confirmation to proceed

**Steps:**

1. **Archive v3 branch reference** (optional safety measure):

   ```bash
   # Create tag pointing to final v3 commit
   git checkout v3
   git tag archive/v3-final-state
   git push origin archive/v3-final-state
   ```

2. **Delete v3 branch**:

   ```bash
   # Delete remote branch
   git push origin --delete v3
   
   # Delete local branch
   git branch -D v3
   ```

3. **Verify deletion**:
   - [ ] `v3` branch no longer appears in GitHub UI
   - [ ] Archive tag exists (if created): `archive/v3-final-state`
   - [ ] No broken CI workflows referencing deleted branch

**Validation criteria:**

- [ ] `v3` branch deleted from remote
- [ ] Archive tag exists for historical reference
- [ ] No broken workflows or scripts

**Estimated time**: 30 minutes - 1 hour

### Phase 6: Post-Migration Validation

**Objective**: Comprehensive validation that new branching strategy works correctly.

**Testing scenarios:**

1. **v2 maintenance workflow**:
   - [ ] Create test PR targeting `v2` branch
   - [ ] Verify CI runs correctly
   - [ ] Verify version detection shows v2.x.x
   - [ ] Test merge process
   - [ ] Simulate patch release from `v2` (dry-run)

2. **v3 development workflow**:
   - [ ] Create test PR targeting `main` branch
   - [ ] Verify CI runs correctly
   - [ ] Verify version detection shows v3.x.x
   - [ ] Test merge process
   - [ ] Simulate release from `main` (dry-run)

3. **Documentation verification**:
   - [ ] Documentation site builds from `main`
   - [ ] All links resolve correctly
   - [ ] No references to deleted `v3` branch

4. **External validation**:
   - [ ] Clone repository fresh and verify default branch is `main`
   - [ ] Verify GitHub UI shows correct default
   - [ ] Check community documentation (if any) references `main`

**Validation criteria:**

- [ ] Both branches support full development workflow
- [ ] Deployments configured correctly for both branches
- [ ] All documentation accurate
- [ ] Team can work normally on `main`

**Estimated time**: 2-3 hours

## Risk Assessment

### High Risk Items

1. **CI/CD workflows fail after migration**
   - **Mitigation**: Update and test workflows in Phase 1 BEFORE migration
   - **Rollback**: Revert workflow changes, reset branches

2. **Version detection breaks**
   - **Mitigation**: Test version calculation in Phase 1
   - **Rollback**: Fix versioning config, may need emergency release

### Medium Risk Items

1. **Open PRs disrupted**
   - **Mitigation**: Close/retarget all PRs before migration (except #1014), announce timeline
   - **Impact**: Contributors need to rebase/retarget (manageable)

2. **External documentation references break**
   - **Mitigation**: Search for hardcoded branch references, update docs
   - **Impact**: Some 404s possible, fixable post-migration

### Low Risk Items

1. **Local developer confusion**
   - **Mitigation**: Clear announcement, update contributor docs
   - **Impact**: Minimal - developers can delete local `v3` and work on `main`

2. **Merge conflicts during v3 → main merge**
   - **Status**: PR #1014 shows `mergeable_state: "clean"` - no conflicts expected
   - **Impact**: None expected (branches kept in sync)

## Communication Plan

### Pre-Migration (1 week before)

- [ ] **GitHub Discussion**: Announce migration timeline and rationale
- [ ] **PR Comment**: Notify contributors with open PRs to hold or retarget
- [ ] **Documentation**: Update contributing guide with migration notice

### During Migration (day of)

- [ ] **GitHub Issue**: Create tracking issue for migration progress
- [ ] **Status Updates**: Post updates as each phase completes
- [ ] **Incident Response**: Document any issues encountered

### Post-Migration (day after)

- [ ] **GitHub Discussion**: Announce successful completion
- [ ] **Update Documentation**: Remove migration notices, confirm branch structure
- [ ] **Monitor**: Watch for issues from community for 48 hours

## Rollback Plan

**If migration must be reversed** (within 24 hours):

1. **Reset main to v2 state**:

   ```bash
   git checkout main
   git reset --hard <documented-main-sha>
   git push origin main --force
   ```

2. **Restore default branch to v3**:
   - GitHub Settings → Branches → Change default to `v3`

3. **Delete v2 branch** (if created):

   ```bash
   git push origin --delete v2
   ```

4. **Revert CI/CD changes**:
   - Roll back workflow files to pre-migration state

5. **Communicate rollback**:
   - Post to tracking issue/discussion explaining what happened

## Success Criteria

**Migration is considered successful when:**

- [ ] `main` branch contains v3 sources and is default branch
- [ ] `v2` branch exists for maintenance with working CI/CD
- [ ] `v3` branch deleted (archived as tag)
- [ ] CI/CD deploys correctly from both `main` and `v2`
- [ ] Version detection works for v2.x and v3.x
- [ ] All documentation references updated
- [ ] Team can work normally on `main`
- [ ] No broken workflows or deployment pipelines
- [ ] 48-hour post-migration monitoring shows no issues

## Timeline Estimate

**Total effort**: 10-16 hours spread across 2-3 days

| Phase | Time | Dependencies |
|-------|------|--------------|
| Phase 0: Pre-migration validation | Gates only | v3.0 stable ready |
| Phase 1: CI/CD updates | 4-6 hours | None |
| Phase 2: Create v2 branch | 1-2 hours | Phase 1 complete |
| Phase 3: Merge v3 to main | 1-2 hours | Phase 2 complete |
| Phase 4: Update repo settings | 1-2 hours | Phase 3 complete |
| Phase 5: Delete v3 branch | 0.5-1 hour | 48hr wait after Phase 3 |
| Phase 6: Post-migration validation | 2-3 hours | Phase 5 complete |

**Recommended schedule:**

- **Day 1**: Phases 0-2 (Pre-validation + CI/CD + create v2 branch)
- **Day 2**: Phases 3-4 (Migration execution + repo settings)
- **Day 4**: Phases 5-6 (Delete v3 + final validation after 48hr wait)

## Related Work

- **Blocking**: v3.0.0 stable release (#1585, community feedback from streaming-indicators.plan.md)
- **Related**: Documentation site migration (#1739) - should happen AFTER branching migration
- **Related**: File reorganization (#1810-#1813) - should happen AFTER branching migration

---
Last updated: January 3, 2026
