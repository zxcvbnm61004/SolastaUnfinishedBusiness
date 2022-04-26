﻿using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionFeatureSetBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionFeatureSet
        where TBuilder : FeatureDefinitionFeatureSetBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder ClearFeatureSet()
        {
            Definition.ClearFeatureSet();
            return (TBuilder)this;
        }

        public TBuilder SetFeatureSet(params FeatureDefinition[] featureDefinitions)
        {
            return SetFeatureSet(featureDefinitions.AsEnumerable());
        }

        public TBuilder SetFeatureSet(IEnumerable<FeatureDefinition> featureDefinitions)
        {
            Definition.SetFeatureSet(featureDefinitions);
            Definition.FeatureSet.Sort(Sorting.CompareTitle);
            return (TBuilder)this;
        }

        public TBuilder AddFeatureSet(params FeatureDefinition[] featureDefinitions)
        {
            return AddFeatureSet(featureDefinitions.AsEnumerable());
        }

        public TBuilder AddFeatureSet(IEnumerable<FeatureDefinition> featureDefinitions)
        {
            Definition.AddFeatureSet(featureDefinitions);
            Definition.FeatureSet.Sort(Sorting.CompareTitle);
            return (TBuilder)this;
        }

        public TBuilder SetEnumerateInDescription(bool value)
        {
            Definition.SetEnumerateInDescription(value);
            return (TBuilder)this;
        }

        public TBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
        {
            Definition.SetMode(mode);
            return (TBuilder)this;
        }

        public TBuilder SetUniqueChoices(bool uniqueChoice)
        {
            Definition.SetUniqueChoices(uniqueChoice);
            return (TBuilder)this;
        }
    }

    public class FeatureDefinitionFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder<FeatureDefinitionFeatureSet, FeatureDefinitionFeatureSetBuilder>
    {
        #region Constructors

        protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }

    public class FeatureDefinitionFeatureSetWithPreRequisitesBuilder : FeatureDefinitionFeatureSetBuilder<FeatureDefinitionFeatureSetWithPreRequisites, FeatureDefinitionFeatureSetWithPreRequisitesBuilder>
    {
        #region Constructors

        protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(FeatureDefinitionFeatureSetWithPreRequisites original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(FeatureDefinitionFeatureSetWithPreRequisites original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionFeatureSetWithPreRequisitesBuilder SetValidators(params Func<bool>[] validators)
        {
            Definition.Validators.AddRange(validators);

            return this;
        }
    }

    public class FeatureDefinitionFeatureSetRemovalBuilder : FeatureDefinitionFeatureSetBuilder<FeatureDefinitionFeatureSetRemoval, FeatureDefinitionFeatureSetRemovalBuilder>
    {
        #region Constructors

        protected FeatureDefinitionFeatureSetRemovalBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetRemovalBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionFeatureSetRemovalBuilder(FeatureDefinitionFeatureSetRemoval original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetRemovalBuilder(FeatureDefinitionFeatureSetRemoval original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionFeatureSetRemovalBuilder SetDynamicFeatureSetFunc(Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> func)
        {
            Definition.DynamicFeatureSet = func;

            return this;
        }
    }

    public class FeatureDefinitionFeatureSetUniqueAcrossBuilder : FeatureDefinitionFeatureSetBuilder<FeatureDefinitionFeatureSetUniqueAcross, FeatureDefinitionFeatureSetUniqueAcrossBuilder>
    {
        #region Constructors

        protected FeatureDefinitionFeatureSetUniqueAcrossBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetUniqueAcrossBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionFeatureSetUniqueAcrossBuilder(FeatureDefinitionFeatureSetUniqueAcross original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetUniqueAcrossBuilder(FeatureDefinitionFeatureSetUniqueAcross original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionFeatureSetUniqueAcrossBuilder SetDynamicFeatureSetFunc(Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> func)
        {
            Definition.DynamicFeatureSet = func;

            return this;
        }
    }
}
